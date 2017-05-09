using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;

namespace Scheduler
{
    public partial class SchedulerView : UserControl
    {
        ToolTip tooltip = new ToolTip();
        public SchedulerView()
        {
            InitializeComponent();
            LeftContainer.SplitterMoved += new SplitterEventHandler(SplitterMoved);
            RightContainer.SplitterMoved+=new SplitterEventHandler(SplitterMoved);
            LeftContainer.SplitterMoving += new SplitterCancelEventHandler(SplitterMoving);
            RightContainer.SplitterMoving += new SplitterCancelEventHandler(SplitterMoving);
            TaskContainer.HorizontalScroll.Enabled = false;
            TaskContainer.HorizontalScroll.Visible = false;
            TaskContainer.Scroll += new ScrollEventHandler(TaskContainer_Scroll);
            ResourceContainer.Scroll += new ScrollEventHandler(ResourceContainer_Scroll);

            TaskNamePanel.Paint += new PaintEventHandler(PaintTaskNamePanel);
            TaskPanel.Paint += new PaintEventHandler(PaintTaskPanel);
            ResourceNamePanel.Paint += new PaintEventHandler(PaintResourceNamePanel);
            ResourcePanel.Paint += new PaintEventHandler(PaintResourcePanel);
            TaskPanel.MouseMove += new MouseEventHandler(TaskPanel_MouseMove);
            ResourcePanel.MouseMove += new MouseEventHandler(ResourcePanel_MouseMove);

            TaskContainer.SizeChanged += new EventHandler(OnSizeChanged);
            ResourceContainer.SizeChanged += new EventHandler(OnSizeChanged);
            TimePanel.Paint += new PaintEventHandler(PaintTimePanel);
        }

        void PaintTimePanel(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle clips = e.ClipRectangle;
            int linePosY = 5;
            int MinorLineHeight = 4;
            int MajorLineHeight = 9;
            int x = TaskPanel.Location.X;
            g.DrawLine(Pens.Black, clips.X+nameColumnWidth, linePosY, clips.Width, linePosY);
            int maxCount = (int)((TimePanel.Width - TaskPanel.Location.X-nameColumnWidth) * 1.0 / (MinorTimeUnit * scaleFactor))+1;
            int start=(int)((0-TaskPanel.Location.X) / (MinorTimeUnit * scaleFactor));
            for (int i =start ; i <= maxCount; i++)
            {
                if (i < 0)
                    continue;
                float xx = (int)( i * MinorTimeUnit * scaleFactor + TaskPanel.Location.X + nameColumnWidth);
                if (xx < nameColumnWidth)
                    continue;
                if (i % TimeUnitCountPerMajorUnit == 0)
                {
                    Pen p = new Pen(Color.Black, 1);

                    g.DrawLine(p, xx, linePosY, xx, linePosY + MajorLineHeight);
                    g.DrawString(formatTimeScaleText(i * MinorTimeUnit), SystemFonts.DefaultFont, Brushes.Black,
                        xx, linePosY + MajorLineHeight + 3);
                }
                else
                {
                    g.DrawLine(Pens.Black, xx, linePosY, xx, linePosY + MinorLineHeight);
                }
            }
            g.FillRectangle(new SolidBrush(TimePanel.BackColor), new Rectangle(0, 0, nameColumnWidth, TimePanel.Height));
        }

        void OnSizeChanged(object sender, EventArgs e)
        {
            RefreshState();
        }
        IRuntimeActivity lastResourceActivity = null;
        void ResourcePanel_MouseMove(object sender, MouseEventArgs e)
        {
            int unit = -1;
            string name = null;
            int taskIndex = -1;
            IRuntimeActivity res = GetResourceActivityAtPosition(e.X, e.Y, out name, out unit, out taskIndex);
            if (res != null)
            {
                if (!res.Equals(lastResourceActivity))
                {
                    string t = string.Format("Resource {0}({1})\n Duration: {2}\n Ellapse: {3} / Remainning: {4}\nActivity: {5}\nTask: {6}",
                        name, unit, formatTimeScaleText(res.Duration), FormatTime(res.PlannedStart), FormatTime(res.PlannedStart + res.PlannedDuration), res.Name, tasks[taskIndex].Name);
                    lastResourceActivity = res;
                    tooltip.SetToolTip(ResourcePanel, t);
                }
            }
            else
            {
                lastResourceActivity = null;
                tooltip.SetToolTip(ResourcePanel, null);
            }
        }
        IRuntimeActivity GetResourceActivityAtPosition(int x, int y, out string res, out int unit, out int taskIndex)
        {
            unit = -1;
            taskIndex = -1;
            res = null;
            if (tasks == null || resources == null)
                return null;
            int row = (y+1) / RowHeight;
            int total = -1;
            for (int i = 0; i < resources.Length; i++)
            {
                for (int j = 0; j < resources[i].Count; j++)
                {
                    total++;
                    if (total == row)
                    {
                        unit = j;
                        res = resources[i].Name;
                    }
                }
            }
            for (int i = 0; i < tasks.Length; i++)
            {
                for (int j = 0; j < tasks[i].Activities.Length; j++)
                {
                    if (tasks[i].Activities[j].PlannedStart * scaleFactor <= x &&
                        tasks[i].Activities[j].PlannedStart * scaleFactor + tasks[i].Activities[j].PlannedDuration * scaleFactor > x)
                    {
                        foreach (string key in tasks[i].Activities[j].Reservations.Keys)
                        {
                            if (key.Equals(res))
                            {
                                int[] us=tasks[i].Activities[j].Reservations[key];
                                for(int m=0;m<us.Length;m++)
                                    if (us[m] == unit)
                                    {
                                        taskIndex = i;
                                        return tasks[i].Activities[j];
                                    }
                            }
                        }
                    }
                }
            }
            return null;
        }
        IRuntimeActivity lastActivity = null;
        string FormatTime(int offset)
        {
            DateTime t2 = (scheduler.ExecuteStartTime != null) ? scheduler.ExecuteStartTime.Value.AddSeconds(offset) : DateTime.Now.AddSeconds(offset);
            return t2.Hour + ":" + t2.Minute + ":" + t2.Second;
        }
        void TaskPanel_MouseMove(object sender, MouseEventArgs e)
        {
            int taskIndex=-1;
            IRuntimeActivity act = GetActivityAtPosition(e.X, e.Y, out taskIndex);
            if (act != null)
            {
                if (!act.Equals(lastActivity))
                {
                    string res = "";
                    foreach (string key in act.Reservations.Keys)
                    {
                        for (int i = 0; i < act.Reservations[key].Length; i++)
                        {
                            res += "    " + key + "(" + (act.Reservations[key][i]) + ")\n";
                        }
                    }
                    if (res.Length == 0)
                        res = "no resource used";
                    string str = string.Format("Activity: {0}\nStatus: {6}\n Duration: {1}\n Ellapse: {2} / Remaining: {3}\n Resource:\n{4}Task: {5}",
                        act.Name, formatTimeScaleText(act.Duration), FormatTime(act.PlannedStart), 
                        FormatTime(act.PlannedStart + act.PlannedDuration), res, tasks[taskIndex].Name, act.Status);
                    tooltip.SetToolTip(TaskPanel, str);
                    lastActivity = act;
                }
            }
            else
            {
                tooltip.SetToolTip(TaskPanel, null);
                lastActivity = null;
            }
        }

        IRuntimeActivity GetActivityAtPosition(int x, int y, out int taskIndex)
        {
            taskIndex = -1;
            if (tasks == null || resources == null)
                return null;
            int tindex = (y - TimeRowHeight+1) / RowHeight;
            if (tindex < 0)
                return null;
            if (tindex + 1 > tasks.Length)
                return null;
            for (int i = 0; i < tasks[tindex].Activities.Length; i++)
                if (tasks[tindex].Activities[i].PlannedStart * scaleFactor<= x &&
                    tasks[tindex].Activities[i].PlannedStart * scaleFactor + tasks[tindex].Activities[i].PlannedDuration * scaleFactor > x)
                {
                    taskIndex = tindex;
                    return tasks[tindex].Activities[i];
                }
            return null;
        }
        void PaintResourcePanel(object sender, PaintEventArgs e)
        {
            if (tasks == null || resources == null)
                return;
            scheduler.Lock();
            try
            {
                Dictionary<int, IRuntimeTask> id2task = new Dictionary<int, IRuntimeTask>();

                foreach (IRuntimeTask t in tasks)
                {
                    id2task[t.ID] = t;
                }
                Graphics g = e.Graphics;

                int count = 0;
                Dictionary<string, int> resouceColumns = new Dictionary<string, int>();
                Dictionary<string, IRuntimeResource> name2res = new Dictionary<string, IRuntimeResource>();
                //draw resource lable
                for (int i = 0; i < resources.Length; i++)
                {
                    name2res[resources[i].Name] = resources[i];
                    if (resources[i].Count == 1)
                    {
                        count++;
                        resouceColumns[resources[i].Name + "_0"] = count - 1;
                    }
                    else
                    {
                        for (int j = 0; j < resources[i].Count; j++)
                        {
                            count++;
                            string name = resources[i].Name + "-" + (j + 1);
                            if (!resources[i].IsUnitEnabled(j))
                            {
                                name = name + "(disabled)";
                            }
                            resouceColumns[resources[i].Name + "_" + j] = count - 1;
                        }
                    }
                }

                for (int i = 0; i < tasks.Length; i++)
                {
                    if (!IsTaskOK(tasks[i]))
                        continue;
                    for (int j = 0; j < tasks[i].Activities.Length; j++)
                    {
                        IRuntimeActivity activity = tasks[i].Activities[j];

                        foreach (string key in activity.Reservations.Keys)
                        {
                            IRuntimeResource r = name2res[key];
                            int[] units = activity.Reservations[key];
                            for (int m = 0; m < units.Length; m++)
                            {
                                string nn = key + "_" + units[m];
                                if (units[m] >= 0 && resouceColumns.ContainsKey(nn))
                                {
                                    int rc = resouceColumns[nn];
                                    //draw activity resource reservation
                                    g.FillRectangle(new SolidBrush(r.Color), new RectangleF(activity.PlannedStart * ScaleFactor, rc * RowHeight, (activity.PlannedDuration * ScaleFactor), RowHeight - 1));

                                    if ((activity.PlannedDuration * ScaleFactor) > 1)
                                        g.DrawString(activity.Name, SystemFonts.DefaultFont, Brushes.Black, new RectangleF(activity.PlannedStart * ScaleFactor, rc * RowHeight, (activity.PlannedDuration * ScaleFactor), RowHeight - 1));
                                }
                            }
                        }
                    }
                }
                //draw current Line
                g.DrawLine(Pens.Red, scheduler.Current * scaleFactor, 0, scheduler.Current * scaleFactor, ResourcePanel.Height);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            finally{
                scheduler.Unlock();
            }
        }

        void PaintResourceNamePanel(object sender, PaintEventArgs e)
        {
            if (tasks == null || resources == null)
                return;
            Graphics g = e.Graphics;
            int index = 0;
            for (int i = 0; i < resources.Length; i++)
            {
                for (int j = 0; j < resources[i].Count; j++)
                {
                    if (resources[i].Count == 1)
                    {
                        g.DrawString(resources[i].Name, SystemFonts.DefaultFont, Brushes.Black, new Rectangle(0, RowHeight * index, nameColumnWidth, RowHeight));
                        index++;
                    }
                    else
                    {
                        string name = resources[i].Name + "-" + (j + 1);
                        if (!resources[i].IsUnitEnabled(j))
                        {
                            name = name + "(x)";
                        }
                        g.DrawString(name, SystemFonts.DefaultFont, Brushes.Black, new Rectangle(0, RowHeight * index, nameColumnWidth, RowHeight));
                        index++;
                    }
                }
            }
        }

        string formatTimeScaleText(int seconds)
        {
            if (seconds < 60)
            {
                return ((seconds < 10) ? ("0" + seconds) : ""+seconds)+"s";
            }
            if (seconds < 3600)
            {
                int m = seconds / 60;
                int s = seconds % 60;
                string mt = (m < 10) ? "0" + m : "" + m;
                string st = (s < 10) ? "0" + s : "" + s;
                return mt+ "m" + st+"s";
            }
            else
            {
                int h = seconds / 3600;
                int m = (seconds%3600) / 60;
                int s = seconds % 60;
                string ht = (h < 10) ? "0" + h : "" + h;
                string mt = (m < 10) ? "0" + m : "" + m;
                string st = (s < 10) ? "0" + s : "" + s;
                return ht+":" + mt + ":" + st;
            }
        }
        bool IsTaskOK(IRuntimeTask t)
        {
            return !( t.Status == Status.NotSchedulable || t.Status == Status.NotScheduled || t.Status == Status.Unscheduled);
        }
        void PaintTaskPanel(object sender, PaintEventArgs e)
        {
            if (scheduler == null)
                return;
            if (tasks == null || resources == null)
                return;
            scheduler.Lock();
            try
            {
                Graphics g = e.Graphics;
                Dictionary<int, IRuntimeTask> id2task = new Dictionary<int, IRuntimeTask>();

                foreach (IRuntimeTask t in tasks)
                {
                    id2task[t.ID] = t;
                }

                for (int i = 0; i < tasks.Length; i++)
                {
                    if (!IsTaskOK(tasks[i]))
                        continue;
                    int start = 0;
                    if (i % 2 == 1)
                    {
                        g.FillRectangle(backBrush, new RectangleF(e.ClipRectangle.X, TimeRowHeight + i * RowHeight, e.ClipRectangle.Width, RowHeight));
                    }
                    for (int j = 0; j < tasks[i].Activities.Length; j++)
                    {
                        IRuntimeActivity activity = tasks[i].Activities[j];
                        //draw activity duration
                        //if (j > 0 && tasks[i].Activities[j - 1].Status != Status.Cancelled)
                        {
                            g.FillRectangle(new SolidBrush(activity.Color),
                                new RectangleF(start + activity.PlannedStart * ScaleFactor, TimeRowHeight + i * RowHeight, activity.PlannedDuration * ScaleFactor, RowHeight - 1));
                            if (activity.Status == Status.Cancelled)
                                g.FillRectangle(new SolidBrush(Color.FromArgb(120, Color.Black)),
                                    new RectangleF(start + activity.PlannedStart * ScaleFactor, TimeRowHeight + i * RowHeight, activity.PlannedDuration * ScaleFactor, RowHeight - 1));
                            if (activity.PlannedDuration * ScaleFactor >= 2)
                                g.DrawString(activity.Name, SystemFonts.DefaultFont, Brushes.Black,
                                new RectangleF(start + activity.PlannedStart * ScaleFactor, TimeRowHeight + i * RowHeight, activity.PlannedDuration * ScaleFactor, RowHeight - 1));

                        }
                    }
                }
                //draw relation
                for (int i = 0; i < tasks.Length; i++)
                {
                    if (!IsTaskOK(tasks[i]))
                        continue;
                    foreach (IRuntimeTask pt in tasks[i].TasksRunAfterEnd)
                    {
                        if (!IsTaskOK(pt))
                            continue;
                        PointF endPoint = new PointF(ScaleFactor * (tasks[i].Activities[0].PlannedStart), TimeRowHeight + RowHeight * i + RowHeight * 0.5f);

                        int j = pt.ID - 1;
                        PointF startPoint = new PointF(ScaleFactor * (pt.Activities[pt.Activities.Length - 1].PlannedDuration + pt.Activities[pt.Activities.Length - 1].PlannedStart), TimeRowHeight + RowHeight * j + RowHeight * 0.5f);
                        if (endPoint.X - startPoint.X <= 3)
                        {
                            int w = 10;
                            PointF point2 = new PointF(startPoint.X + w, startPoint.Y);
                            PointF point3 = new PointF(startPoint.X + w, startPoint.Y / 2.0f + endPoint.Y / 2.0f);
                            PointF point4 = new PointF(endPoint.X - w, startPoint.Y / 2.0f + endPoint.Y / 2.0f);
                            PointF point5 = new PointF(endPoint.X - w, endPoint.Y);
                            g.DrawLines(Pens.Black, new PointF[] { startPoint, point2, point3, point4, point5, endPoint });
                        }
                        else
                        {
                            PointF point2 = new PointF(startPoint.X / 2.0f + endPoint.X / 2.0f, startPoint.Y);
                            PointF point3 = new PointF(startPoint.X / 2.0f + endPoint.X / 2.0f, endPoint.Y);
                            g.DrawLine(Pens.Black, startPoint, point2);
                            g.DrawLine(Pens.Black, point2, point3);
                            g.DrawLine(Pens.Black, point3, endPoint);
                        }
                        g.DrawLine(Pens.Black, endPoint, new PointF(endPoint.X - 3, endPoint.Y - 3));
                        g.DrawLine(Pens.Black, endPoint, new PointF(endPoint.X - 3, endPoint.Y + 3));
                    }
                    foreach (IRuntimeTask pt in tasks[i].TasksRunAfterStart)
                    {
                        PointF endPoint = new PointF(ScaleFactor * (tasks[i].PlannedStart), TimeRowHeight + RowHeight * i + RowHeight * 0.5f);

                        int j = pt.ID - 1;
                        PointF startPoint = new PointF(ScaleFactor * (pt.PlannedStart), TimeRowHeight + RowHeight * j + RowHeight * 0.5f);
                        Pen p = new Pen(Color.Black, 1);
                        p.DashPattern = new float[] { 5f, 4f, 3f, 4f };
                        //p.EndCap = LineCap.ArrowAnchor;
                        if (endPoint.X - startPoint.X <= 3)
                        {
                            int w = 10;
                            PointF point2 = new PointF(startPoint.X + w, startPoint.Y);
                            PointF point3 = new PointF(startPoint.X + w, startPoint.Y / 2.0f + endPoint.Y / 2.0f);
                            PointF point4 = new PointF(endPoint.X - w, startPoint.Y / 2.0f + endPoint.Y / 2.0f);
                            PointF point5 = new PointF(endPoint.X - w, endPoint.Y);
                            g.DrawLines(p, new PointF[] { startPoint, point2, point3, point4, point5, endPoint });
                        }
                        else
                        {
                            PointF point1 = new PointF(startPoint.X - 5, startPoint.Y);
                            PointF point2 = new PointF(startPoint.X - 5, endPoint.Y);
                            PointF point3 = new PointF(startPoint.X / 2.0f + endPoint.X / 2.0f, endPoint.Y);
                            g.DrawLines(p, new PointF[] { startPoint, point1, point2, endPoint });
                        }
                        g.DrawLine(Pens.Black, endPoint, new PointF(endPoint.X - 3, endPoint.Y - 3));
                        g.DrawLine(Pens.Black, endPoint, new PointF(endPoint.X - 3, endPoint.Y + 3));
                    }
                }

                //draw current Line
                g.DrawLine(Pens.Red, scheduler.Current * scaleFactor, 0, scheduler.Current * scaleFactor, TaskPanel.Height);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                scheduler.Unlock();
            }
        }
        SolidBrush backBrush=new SolidBrush(Color.FromArgb(40, Color.Yellow));
        void PaintTaskNamePanel(object sender, PaintEventArgs e)
        {
            if (tasks == null || resources == null)
                return;
            Graphics g = e.Graphics;
            if (tasks != null)
            {
                for (int i = 0; i < tasks.Length; i++)
                {
                    if (i % 2 == 1)
                    {
                        g.FillRectangle(backBrush, new RectangleF(e.ClipRectangle.X, TimeRowHeight + i * RowHeight, e.ClipRectangle.Width, RowHeight));
                    }
                    g.DrawString(tasks[i].Name+"("+tasks[i].ID+")", SystemFonts.DefaultFont, Brushes.Black, new Rectangle(0, TimeRowHeight + RowHeight * i, nameColumnWidth, RowHeight));
                }
            }
        }

        void ResourceContainer_Scroll(object sender, ScrollEventArgs e)
        {
            ResourceNamePanel.Top = ResourcePanel.Top;
            TaskPanel.Left = ResourcePanel.Left;
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
                TimePanel.Refresh();
            ShowScrollBar(TaskContainer.Handle, (int)ScrollBarDirection.SB_HORZ, false);
        }

        void TaskContainer_Scroll(object sender, ScrollEventArgs e)
        {
            TaskNamePanel.Top= TaskPanel.Location.Y;
        }
        IRuntimeTask[] tasks ;
        IRuntimeResource[] resources;
        IRuntimeScheduler scheduler;
        
        public void Init(IRuntimeScheduler scheduler)
        {
            this.scheduler = scheduler;
            //scheduler.StateChanged += new EventHandler<SchedulerStateChangeEventArg>(scheduler_StateChanged);
            //scheduler.TaskScheduleStarted += new EventHandler<SchedulerStateChangeEventArg>(scheduler_TaskScheduleStarted);
            //scheduler.TaskScheduleCompleted += new EventHandler<SchedulerStateChangeEventArg>(scheduler_TaskScheduleCompleted);
        }

        void scheduler_TaskScheduleCompleted(object sender, SchedulerStateChangeEventArg e)
        {
        }

        void scheduler_TaskScheduleStarted(object sender, SchedulerStateChangeEventArg e)
        {
        }

        void scheduler_StateChanged(object sender, SchedulerStateChangeEventArg e)
        {
            tasks=scheduler.Tasks;
            resources=scheduler.Resources;
            RefreshState();
        }
        void RefreshState(){
            if (tasks == null || resources == null)
                return;
            GetTimeUnit(out MinorTimeUnit, out TimeUnitCountPerMajorUnit);
            int width = 0;
            for (int i = 0; i < tasks.Length; i++)
            {
                width = Math.Max(width, (int)((tasks[i].PlannedDuration + tasks[i].PlannedStart) * ScaleFactor));
            }
            int rowCount = tasks.Length+1;
            width = width + 200;
            width = Math.Max(width, TaskContainer.Width);

            int height = TimeRowHeight + rowCount * RowHeight;
            height = Math.Max(height, TaskContainer.Height);
            TaskPanel.Size = new Size(width, height);

            TaskNamePanel.Size = new Size(
                Math.Max(nameColumnWidth, TaskNamePanel.Width),
                Math.Max(RowHeight * tasks.Length + TimeRowHeight, TaskNamePanel.Height));

            rowCount = 0;
            for (int i = 0; i < resources.Length; i++)
                rowCount += resources[i].Count;
            ResourceNamePanel.Size = new Size(
                Math.Max(nameColumnWidth, ResourceNamePanel.Width),
                Math.Max(RowHeight * rowCount, ResourceNamePanel.Height));
            width = Math.Max(width, ResourceContainer.Width);
            height = Math.Max(RowHeight * rowCount, ResourceContainer.Height);
            ResourcePanel.Size = new Size(
                width,
                height);

            TaskContainer.HorizontalScroll.Enabled = false;
            TaskContainer.HorizontalScroll.Visible = false;

            LeftContainer.Width = nameColumnWidth;
            
            TaskPanel.Refresh();
            TaskNamePanel.Refresh();
            ResourceNamePanel.Refresh();
            ResourcePanel.Refresh();
            TimePanel.Refresh();
            ShowScrollBar(TaskContainer.Handle, (int)ScrollBarDirection.SB_HORZ, false);
        }
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);

        private enum ScrollBarDirection
        {
            SB_HORZ = 0,
            SB_VERT = 1,
            SB_CTL = 2,
            SB_BOTH = 3
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            ShowScrollBar(TaskContainer.Handle, (int)ScrollBarDirection.SB_HORZ, false);
            base.WndProc(ref m);
        }
        float scaleFactor = 1.5f;
        public float ScaleFactor
        {
            get { return scaleFactor; }
            set {
                if (value <= 0)
                    return;
                scaleFactor = value; RefreshState(); }
        }
        int MinorTimeUnit=60;
        int TimeUnitCountPerMajorUnit=10;
        int RowHeight = 18;
        int nameColumnWidth = 100;
        int TimeRowHeight = 0;

        void GetTimeUnit(out int minorUnit, out int minorCount)
        {
            
            int[] count = { 5, 5, 5, 3,  5,  4,  3,   4,  5,  5, 5, 5, 4, 4, 4, 5, 5, 5};
            int[] units = { 2, 4, 5, 10, 12, 15, 20, 30, 60, 120, 240, 300, 450, 600, 900, 1200, 1800, 2400};
            int prefferedUnitWidth = 120;
            int[] width = new int[units.Length];
            for (int i = 0; i < units.Length; i++)
                width[i] = (int)Math.Abs(units[i]*count[i] * scaleFactor - prefferedUnitWidth);
            int index = 0;
            int min = width[index];
            for (int i = 1; i < units.Length; i++)
            {
                if (width[i] < min)
                {
                    min = width[i];
                    index = i;
                }
            }
            minorUnit = units[index];
            minorCount = count[index];
        }
        void SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (sender.Equals(LeftContainer))
            {
                RightContainer.SplitterDistance = LeftContainer.SplitterDistance;
            }
            else if (sender.Equals(RightContainer))
                LeftContainer.SplitterDistance = RightContainer.SplitterDistance;
        }


        void SplitterMoving(object sender, SplitterCancelEventArgs e)
        {
            e.Cancel = false;
        }

        private void OnPanelSizeChanged(object sender, EventArgs e)
        {
            //RefreshState();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (scheduler != null)
            {
                tasks = scheduler.Tasks;
                resources = scheduler.Resources;
                RefreshState();
            }
        }
    }
}
