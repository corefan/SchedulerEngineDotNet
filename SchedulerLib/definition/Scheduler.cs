using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Drawing.Drawing2D;

namespace Scheduler
{
    public class Scheduler<T, K>  : IRuntimeScheduler, IScheduler<K> where T:ScheduleTask, new()
    {
        internal class _Resource : IResource
        {
            public string Name
            {
                get;
                set;
            }

            public int Count
            {
                get;
                set;
            }

            public System.Drawing.Color Color
            {
                get;
                set;
            }
        }
        public void Register(string name, int count, int color)
        {
            _Resource r = new _Resource();
            r.Name = name;
            r.Count = count;
            r.Color = Color.FromArgb(color);
            this.Register(r);
        }
        protected List<ScheduleResource> _resources = new List<ScheduleResource>();
        protected LinkedList<T> _tasks = new LinkedList<T>();

        Nullable<DateTime> executeStart = null;
        public int Current
        {
            get
            {
                if (!executeStart.HasValue)
                    return 0;
                else
                    return (int)new TimeSpan(DateTime.Now.Ticks - executeStart.Value.Ticks).TotalSeconds;
            }
        }
        public double CurrentExecuteTime
        {
            get
            {
                if (executeStart == null)
                    return 0;
                else
                    return new TimeSpan(DateTime.Now.Ticks - executeStart.Value.Ticks).TotalSeconds;
            }
        }
        public Nullable<DateTime> ExecuteStartTime
        {
            get { return executeStart; }
        }
        public void Register(IResource res)
        {
            if (res is ScheduleResource)
                _resources.Add(res as ScheduleResource);
            else
                _resources.Add(new ScheduleResource(res));
        }
        public IRuntimeResource GetResource(string name)
        {
            for (int i = 0; i < _resources.Count; i++)
                if (name.Equals(_resources[i].Name))
                    return _resources[i].RuntimeResource;
            return null;
        }
        public int TaskCount
        {
            get { return _tasks.Count; }
        }
        public IRuntimeTask GetTask(int id)
        {
                LinkedListNode<T> t = _tasks.First;
                while (t != null)
                {
                    if (t.Value.ID == id)
                        return t.Value.RuntimeTask;
                    t = t.Next;
                }
                return null;
        }
        public IRuntimeResource[] Resources
        {
            get
            {
                IRuntimeResource[] res = new IRuntimeResource[_resources.Count];
                for (int i = 0; i < _resources.Count; i++)
                    res[i] = _resources[i].RuntimeResource;
                return res;
            }
        }
        public IRuntimeTask[] Tasks
        {
            get
            {
                    IRuntimeTask[] t = new IRuntimeTask[_tasks.Count];
                    int i = 0;
                    LinkedListNode<T> node = _tasks.First;
                    while (node != null)
                    {
                        t[i] = node.Value.RuntimeTask;
                        node = node.Next;
                        i++;
                    }
                    return t;
            }
        }
        public virtual int Activate(K task)
        {
            return Activate(task, null);
        }
        private int ActivateImpl(T t, K task, Dictionary<int, TaskRelation> relations)
        {
            if (relations != null)
            {
                foreach (int key in relations.Keys)
                {
                    t.Relations[key] = relations[key];
                }
            }
            return ActivateImpl(t, task);
        }
        private int ActivateImpl(T t, K task)
        {
            if (_tasks.Contains(t))
                return -1;
            if (IsRunning)
            {
                List<ScheduleTask> TasksAfter = new List<ScheduleTask>();
                foreach (int id in t.Relations.Keys)
                {
                    TaskRelation tr = t.Relations[id];
                    if (tr == TaskRelation.BeforeRunEnd || tr == TaskRelation.BeforeRunStart)
                    {
                        IRuntimeTask it = GetTask(id);
                        if (it == null)
                            throw new Exception("subsequential task can not be found");
                        Status s = GetTask(id).Status;
                        if (s != Status.NotScheduled && s != Status.Scheduled && s != Status.Unscheduled && s != Status.NotSchedulable)
                            throw new Exception("subsequential task is running, processed");
                    }
                }
            }
            LinkedListNode<T> node = null;
            if (_tasks.Count == 0)
                node = _tasks.AddFirst(t);
            else
                node = _tasks.AddLast(t);
            t.ID = _tasks.Count;
            t.Status = Status.NotScheduled;
            t.Scheduler = this;
            t.SetTask(task);
            if (IsRunning)
            {
                Schedule();
            }
            return _tasks.Count;
        }
        public virtual int Activate(K task, Dictionary<int, TaskRelation> relations)
        {
            T t = new T();
            if (relations != null)
            {
                foreach (int key in relations.Keys)
                {
                    t.Relations[key] = relations[key];
                }
            }
            return ActivateImpl(t, task);
        }
        public bool IsRunning
        {
            get
            {
                return executeStart != null && _runThread != null && _runThread.IsAlive;
            }
        }
        Thread _runThread;
        public void Run()
        {
            if (_runThread != null && _runThread.IsAlive)
                return;
            if(!executeStart.HasValue)
                executeStart = DateTime.Now;
            _runThread = new Thread(new ThreadStart(CheckThread));
            _runThread.Start();
        }
        bool CheckTaskStartPoint(ScheduleTask t)
        {
            ScheduleActivity ac = t.Activities[0];
            bool canStart = true;
            foreach (LinkedListNode<UnitReservation> ur in ac.UnitReservations)
            {
                if (ur.Previous != null &&
                    (ur.Previous.Value.Activity.Status != Status.Cancelled && ur.Previous.Value.Activity.Status != Status.Processed && ur.Previous.Value.Activity.WaitingForCompleted!=true))
                    canStart = false;
            }
            
            foreach (ScheduleTask pt in t.TasksRunAfterEnd)
            {
                if (pt.Status != Status.Processed && pt.Status != Status.Cancelled)
                    canStart = false;
            }
             
            
            foreach (ScheduleTask pt in t.TasksRunAfterStart)
            {
                if (pt.Status == Status.Scheduled)
                    canStart = false;
            }
            if (CurrentExecuteTime < t.Activities[0].PlannedStart)
            {
                canStart = false;
            }
            return canStart;
        }
        void RunTask(object t)
        {
            (t as ScheduleTask).Execute(this);
        }
        void CheckThread()
        {
            while (true)
            {
                LinkedListNode<T> node = _tasks.First;
                bool allFinished = true;
                while (node != null)
                {
                    if (node.Value.Status != Status.Processed && node.Value.Status != Status.Cancelled)
                        allFinished = false;
                    if (node.Value.Status == Status.Scheduled && CheckTaskStartPoint(node.Value))
                    {
                        node.Value.ActivityCompleted += new SchedulerEventHandler<ActivityEventArg>(OnActivityCompleted);
                        node.Value.ActivityStarted += new SchedulerEventHandler<ActivityEventArg>(OnActivityStarted);
                        node.Value.ActivityCancelled += new SchedulerEventHandler<ActivityEventArg>(OnActivityCancelled);
                        //node.Value.Execute(this);
                        Thread t = new Thread(new ParameterizedThreadStart(RunTask));
                        node.Value.Status = Status.Running;
                        t.Start(node.Value);
                    }
                    node = node.Next;
                }
                if (allFinished)
                {
                    FireStateChanged();
                    return;
                }
                lock (this)
                {
                    CheckRunState();
                }
                FireStateChanged();
                Thread.Sleep(600);
            }
        }

        void CheckMoveLeftOnActivityComplete(ScheduleActivity completedActivity, int completedActivityOffset)
        {
            LinkedListNode<T> node = _tasks.First;
            int offset = completedActivityOffset;
            int minNextTaskStartTime = int.MaxValue;
            while (node != null)
            {
                if (node.Value.Status == Status.Running)
                {
                    ScheduleActivity curAct = node.Value.CurrentRunningActivity;
                    if (curAct != null)
                    {
                        if (curAct.Status == Status.Running &&
                            curAct.WaitingForCompleted == true)
                        {
                            //int o = curAct.PlannedDuration - Current + curAct.PlannedStart;
                            //if (o < offset)
                            //    offset = o;
                            offset = -1;
                        }
                        else if (curAct.Status == Status.Running)
                        {
                            int o = curAct.PlannedDuration - curAct.Duration;
                            if (o < offset)
                                offset = o;
                        }
                        else if (curAct.Equals(completedActivity))
                        {
                        }
                        else
                        {
                            offset = -1;
                        }
                    }
                    else
                    {
                        offset = -1;
                        //offsetRight = -1;
                    }
                }
                else if (node.Value.Status == Status.Scheduled)
                {
                    int st = node.Value.PlannedStart;
                    if (st < minNextTaskStartTime)
                        minNextTaskStartTime = st;
                }
                node = node.Next;
            }
            if (offset != -1 && minNextTaskStartTime!=-1)
            {
                minNextTaskStartTime = minNextTaskStartTime - Current;
                if (minNextTaskStartTime < offset)
                    offset = minNextTaskStartTime;
            }
            if (offset > 1)
            {
                node = _tasks.First;
                while (node != null)
                {
                    if (node.Value.Status != Status.Processed && node.Value.Status!=Status.Cancelled && node.Value.Status!=Status.Unscheduled
                        && node.Value.Status!=Status.NotSchedulable)
                    {
                        ScheduleTask t = node.Value;
                        for (int i = 0; i < t.Activities.Length; i++)
                        {
                            ScheduleActivity ac = t.Activities[i];
                            if (ac.Status == Status.Running)
                            {
                                ac.PlannedDuration -= offset;
                            }
                            else if (ac.Status != Status.Processed && ac.Status!=Status.Cancelled)
                            {
                                ac.PlannedStart -= offset;
                            }
                            else if (ac.Status == Status.Processed)
                            {
                            }
                        }
                    }
                    node = node.Next;
                }
                CheckConflict("check left move afte task completed");
            }
        }
        void CheckRunState()
        {
            //check run state
            LinkedListNode<T> node = _tasks.First;
            int offset = int.MaxValue;
            int offsetRight = -1;
            int minNextTaskStartTime = int.MaxValue;
            ScheduleActivity moveAct = null;
            while (node != null)
            {
                if (node.Value.Status == Status.Running && node.Value.Activities[0].Status!=Status.Scheduled)
                {
                    ScheduleActivity curAct = node.Value.CurrentRunningActivity;
                    if (curAct != null)
                    {
                        if (curAct.Status == Status.Running &&
                            curAct.WaitingForCompleted == true)
                        {
                            int o1 = curAct.PlannedDuration - Current + curAct.PlannedStart;
                            if (o1 < offset)
                                offset = o1;
                            //offset = -1;
                            //int o = Current - curAct.PlannedStart - curAct.PlannedDuration;
                            //if (o > offsetRight)
                            {
                                //offsetRight = o;
                                //moveAct = curAct;
                            }
                        }
                        else if (curAct.Status == Status.Running)
                        {
                            //problem here
                            int o = curAct.PlannedDuration - curAct.Duration-(Current-curAct.PlannedStart);
                            if (o < offset)
                                offset = o;
                            o = Current - curAct.PlannedStart - curAct.PlannedDuration;
                            if (o > offsetRight)
                            {
                                offsetRight = o;
                                moveAct = curAct;
                            }
                        }   
                        else// if (node.Value.CurrentRunningActivity.Status == Status.Processed)
                        {
                            offset = -1;
                        }   
                    }
                    else
                    {
                        offset = -1;
                        //offsetRight = -1;
                    }
                }
                else if (node.Value.Status == Status.Scheduled || (node.Value.Status==Status.Running && node.Value.Activities[0].Status==Status.Scheduled))
                {
                    int st = node.Value.PlannedStart;
                    if (st < minNextTaskStartTime)
                        minNextTaskStartTime = st;
                    int ostart = Current - node.Value.PlannedStart;
                    if (ostart > offsetRight)
                    {
                        offsetRight = ostart;
                        moveAct = node.Value.Activities[0];
                    }
                }

                node = node.Next;
            }
            if (offset != int.MaxValue)
            {
                minNextTaskStartTime = minNextTaskStartTime - Current;
                if (minNextTaskStartTime < offset)
                    offset = minNextTaskStartTime;
            }
            if (offset > 2 && offset != int.MaxValue)
            {
                 node = _tasks.First;
                while (node != null)
                {
                    if (node.Value.Status != Status.Processed && node.Value.Status!=Status.Cancelled 
                        && node.Value.Status!=Status.NotScheduled && node.Value.Status!=Status.NotSchedulable)
                    {
                        ScheduleTask t = node.Value;
                        for (int i = 0; i < t.Activities.Length; i++)
                        {
                            ScheduleActivity ac = t.Activities[i];
                            if (ac.Status == Status.Running)
                            {
                                ac.PlannedDuration -= offset;
                            }
                            else if (ac.Status != Status.Processed && ac.Status!=Status.Cancelled)
                            {
                                ac.PlannedStart -= offset;
                            }
                            else if (ac.Status == Status.Processed)
                            {
                            }
                        }
                    }
                    node = node.Next;
                }

                CheckConflict("ChckRunState move left");
                //FireStateChanged();
            }
            else if (offsetRight >= 2)
            {
                MoveActivityRight(moveAct, offsetRight);
                CheckConflict("ChckRunState move left");
                //FireStateChanged();
            }
        }

        void OnActivityCancelled(object sender, ActivityEventArg eventArgs)
        {
            ScheduleActivity act = eventArgs.Activity;
            if (act == null)
                return;
            lock (this)
            {
                Console.WriteLine("task {0} - {1} activity {2} is cancelled", act.Task.ID, act.Task.Name, act.Name);
                ScheduleActivity next=act;
                while (next.Next != null)
                    next = next.Next;
                int offset = next.PlannedDuration - Current + next.PlannedStart;
                int planStart = act.PlannedStart;
                if (act.Previous != null)
                {
                    act.PlannedStart = act.Previous.PlannedDuration + act.Previous.PlannedStart;
                }
                act.PlannedDuration = Current - act.PlannedStart;
                act.Status = Status.Cancelled;
                act.Task.Status = Status.Cancelled;
                this.Lock();
                try
                {
                    next = act.Next;
                    while (next != null)
                    {
                        foreach (string res in next.Resources.Keys)
                        {
                            foreach (ScheduleResource r in _resources)
                            {
                                if (res.Equals(r.Name))
                                {
                                    r.RemoveReservationForActivity(next);
                                    break;
                                }
                            }
                        }
                        next.Reservations.Clear();
                        next.UnitReservations.Clear();
                        next.Status = Status.Cancelled;
                        next.PlannedStart = next.Previous.PlannedDuration + next.Previous.PlannedStart;
                        next = next.Next;

                    }
                    if (offset > 0)
                    {
                        //移动activity左移
                        CheckMoveLeftOnActivityComplete(act, offset);
                        CheckConflict("Check move left cancelled");
                    }
                    FireScheduleStarted();
                    foreach (ScheduleTask t in _tasks)
                    {
                        if (t.Status == Status.Scheduled)
                            UnSchedule(t);
                    }
                    SchedulerImpl();
                    FireScheduleCompleted();
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    this.Unlock();
                }
                FireStateChanged();
            }
        }
        protected void OnActivityStarted(object sender, ActivityEventArg eventArgs)
        {
            lock (this)
            {
                ScheduleActivity act = eventArgs.Activity;
                Console.WriteLine("task {0} - {1} activity {2} is started", act.Task.ID, act.Task.Name, act.Name);
                int planStart = act.PlannedStart;
                if (act.Previous != null)
                    act.PlannedStart = act.Previous.PlannedDuration + act.Previous.PlannedStart;
                else
                    act.PlannedStart = Current;
                if(act.Next!=null)
                    act.PlannedDuration += planStart - act.PlannedStart;
                FireStateChanged();
            }
        }


        protected void OnActivityCompleted(object sender, ActivityEventArg eventArgs)
        {
            lock (this)
            {
                ScheduleActivity act = eventArgs.Activity;
                Console.WriteLine("task {0} - {1} activity {2} is completed", act.Task.ID, act.Task.Name, act.Name);
                int offset = act.PlannedDuration - Current + act.PlannedStart;
                act.PlannedDuration = Current - act.PlannedStart;
                if (offset > 0)
                {
                    //移动activity左移
                    CheckMoveLeftOnActivityComplete(act, offset);
                    CheckConflict("after task completed " + act.Task.ID);
                    if (act.Next != null && act.Next.PlannedStart != act.PlannedDuration + act.PlannedStart)
                    {
                        act.Next.PlannedDuration = act.Next.PlannedDuration + (act.Next.PlannedStart - (act.PlannedDuration + act.PlannedStart));
                        act.Next.PlannedStart = act.PlannedDuration + act.PlannedStart;
                    }
                }
                FireStateChanged();
            }
        }

        Dictionary<string, ScheduleResource> _name2res = new Dictionary<string, ScheduleResource>();
        Dictionary<int, ScheduleTask> _id2task = new Dictionary<int, ScheduleTask>();
        ScheduleTask lastScheduledTask = null;
        void UnSchedule(ScheduleTask t)
        {
            if (t.Status == Status.NotSchedulable || t.Status == Status.NotScheduled || t.Status==Status.Unscheduled)
                return;
            if (t.Status != Status.Scheduled)
            {
                throw new Exception("can not unschedule task " + t.ID + ", because it is running, processed");
            }
            
            //unschedule the task must be runned before it
            LinkedListNode<T> node = _tasks.First;
            while (node != null)
            {
                bool AfterT = false;
                foreach (ScheduleTask task in node.Value.TasksRunAfterEnd)
                {
                    if(task.Equals(t))
                        AfterT=true;
                }
                foreach (ScheduleTask task in node.Value.TasksRunAfterStart)
                {
                    if (task.Equals(t))
                        AfterT = true;
                }
                if (AfterT)
                    UnSchedule(node.Value);
                node = node.Next;
            }

            //unschedule 
            for (int i = 0; i < t.Activities.Length; i++)
            {
                Dictionary<ScheduleResource, bool> res = new Dictionary<ScheduleResource, bool>();
                ScheduleActivity act = t.Activities[i];
                foreach (LinkedListNode<UnitReservation> ur in act.UnitReservations)
                {
                    res[ur.Value.Resource] = true;
                }
                foreach (ScheduleResource r in res.Keys)
                    r.RemoveReservationForActivity(act);
                act.Reservations.Clear();
                act.UnitReservations.Clear();
                act.PlannedDuration = -1;
                act.PlannedStart = -1;
                act.Status = Status.Unscheduled;
            }
            t.Status = Status.Unscheduled;
            //Console.WriteLine("task " + t.ID + " is unschedule");
            FireStateChanged();
        }

        //stretch or move activity forward, if ok, return null, else return which activity should be move forward
        private ScheduleActivity ScheduleBackward(ScheduleActivity ac, int endTime)
        {
            bool canSchedule = true;
            int maxDuration = 0;
            int ostart=ac.PlannedStart;
            if (ac.Resources.Count > 0)
            {
                int checkstart=Math.Max(ac.PlannedStart,(int)( endTime - ac.Duration*(1-SchedulerSetting.Instance.DefaultDecreaseRate)));
                for (int start = checkstart ; start >= ostart; start--)
                {
                    ac.PlannedStart = start;
                    ac.PlannedDuration = endTime - start;

                    if (ac.MaxPlannedDuration >= ac.Duration && ac.PlannedDuration > ac.MaxPlannedDuration)
                        break;
                    else if (ac.MaxPlannedDuration < ac.Duration && ac.PlannedDuration > ac.Duration * SchedulerSetting.Instance.MaxPlantimeEnlarge)
                        break;
                    foreach (string key in ac.Resources.Keys)
                    {
                        ScheduleResource trc = _name2res[key];
                        int count = ac.Resources[key];
                        int[] units = null;
                        if ((units = trc.CheckReservation(ac, ac.PlannedStart, ac.PlannedDuration, count)) != null)
                        {
                            if (ac.PlannedDuration> maxDuration)
                                maxDuration = ac.PlannedDuration;
                        }
                        else
                        {
                            canSchedule = false;
                        }
                        if (!canSchedule)
                            break;
                    }
                    if (!canSchedule)
                        break;
                }
            }
            else
            {
                if(ac.MaxPlannedDuration>ac.Duration)
                    maxDuration = Math.Min(ac.MaxPlannedDuration, endTime - ac.PlannedStart);
                else
                    maxDuration=Math.Min(endTime-ac.PlannedStart,(int)(ac.Duration*SchedulerSetting.Instance.MaxPlantimeEnlarge));
            }

            if (canSchedule)
            {
                if (ac.Previous == null && maxDuration > ac.Duration)
                    maxDuration = ac.Duration;
                ac.PlannedDuration = maxDuration;
                ac.PlannedStart = endTime - maxDuration;

                Console.WriteLine("!Make backward reservation for task {3} activity {0} start={1} duration={2} and endtime={4}", 
                    ac.Name, ac.PlannedStart, ac.PlannedDuration, ac.Task.ID, endTime);
                if (ac.Previous != null && ac.Previous.PlannedStart + ac.Previous.PlannedDuration != ac.PlannedStart)
                {
                    return ScheduleBackward(ac.Previous, ac.PlannedStart);
                }
                return null;
            }
            return ac;
        }
        private bool ScheduleForeward(ScheduleActivity ac, int startTime)
        {
            ac.PlannedStart = startTime;
            ac.PlannedDuration = ac.Duration;
            int minPlannedDuration = int.MaxValue;
            int maxNextFreeTime = 0;
            bool canSchedule = true;
            if (ac.Resources.Count > 0)
            {
                foreach (string key in ac.Resources.Keys)
                {
                    ScheduleResource trc = _name2res[key];
                    int count = ac.Resources[key];
                    int nextFreeTimeOffset = -1;
                    int[] units = null;
                    int planedDuration = 0;
                    //增加下一个可用位置的记忆
                    if ((units = trc.CheckReservationForward(ac, ac.PlannedStart, ac.PlannedDuration, count, out planedDuration, out nextFreeTimeOffset)) != null)
                    {
                        if (planedDuration < minPlannedDuration)
                            minPlannedDuration = planedDuration;
                        if (nextFreeTimeOffset > maxNextFreeTime)
                            maxNextFreeTime = nextFreeTimeOffset;
                    }
                    else
                    {
                        if (nextFreeTimeOffset > maxNextFreeTime)
                            maxNextFreeTime = nextFreeTimeOffset;
                        canSchedule = false;
                    }
                }
            }
            else
            {
                minPlannedDuration = ac.Duration;
                maxNextFreeTime = ac.PlannedStart + 1;
            }

            if (canSchedule)
            {
                //make reservation
                ac.PlannedDuration = minPlannedDuration;
                ac.NextAvailableTime = startTime + maxNextFreeTime;
                Console.WriteLine("Make forward reservation for task {3} activity {0} start={1} duration={2}", ac.Name, ac.PlannedStart, ac.PlannedDuration,ac.Task.ID);
                if (ac.Next != null)
                    return ScheduleForeward(ac.Next, ac.PlannedStart + ac.PlannedDuration);
                else
                    return true;
            }
            else
            {
                bool forward = true;
                ScheduleActivity mact = null;
                if (ac.Previous != null)
                    forward = ((mact=ScheduleBackward(ac.Previous, ac.PlannedStart + maxNextFreeTime))==null);
                if (forward)
                {
                     return ScheduleForeward(ac, startTime + maxNextFreeTime);
                }
                else
                {
                    return ScheduleForeward(
                        mact,
                        Math.Max(mact.NextAvailableTime, (mact.PlannedStart + 1)));
                }
            }
        }
        private void DoSchedule(ScheduleTask t, int level = 0)
        {
            if (level >= _tasks.Count)
                throw new Exception("can not schedule the task due to task relations");
            if (t.Status != Status.NotScheduled && t.Status != Status.Unscheduled)
            {
                return;
            }
            int startTime = Current;
            //unschedule the task must be runned before it
            {
                LinkedListNode<T> node = _tasks.First;
                while (node != null)
                {
                    bool AfterT = false;
                    foreach (ScheduleTask task in node.Value.TasksRunAfterEnd)
                    {
                        if (task.Equals(t))
                            AfterT = true;
                    }
                    foreach (ScheduleTask task in node.Value.TasksRunAfterStart)
                    {
                        if (task.Equals(t))
                            AfterT = true;
                    }
                    if (AfterT)
                    {
                        UnSchedule(node.Value);
                    }
                    node = node.Next;
                }
            }
            //end of unschedule
            if (t.TasksRunAfterEnd.Count > 0 || t.TasksRunAfterStart.Count > 0)
            {
                foreach (ScheduleTask task in t.TasksRunAfterStart)
                {
                    level++;
                    if (task.Status == Status.NotScheduled)
                    {
                        DoSchedule(task, level);
                    }
                    startTime = Math.Max(startTime, task.PlannedStart);

                }
                foreach (ScheduleTask task in t.TasksRunAfterEnd)
                {
                    level++;
                    if (task.Status == Status.NotScheduled)
                    {
                        DoSchedule(task, level);
                    }
                    startTime = Math.Max(startTime, task.PlannedStart + task.PlannedDuration);
                }
            }
            if (ScheduleForeward(t.Activities[0], startTime))
            {
                for (int j = 0; j < t.Activities.Length; j++)
                {
                    ScheduleActivity ac = t.Activities[j];
                    foreach (string key in ac.Resources.Keys)
                    {
                        ScheduleResource trc = _name2res[key];
                        int count = t.Activities[j].Resources[key];
                        ac.Reservations[key] = trc.Reserve(ac, ac.PlannedStart, ac.PlannedDuration, count);
                        if (ac.Reservations[key] == null)
                        {
                            throw new Exception("unexpected scheduler error");
                        }
                    }
                    ac.Status = Status.Scheduled;
                }
                t.Status = Status.Scheduled;
            }
            else
            {
                throw new Exception("unexpected scheduler error");
            }
            if (t.Status != Status.Scheduled)
            {
                t.Status = Status.NotSchedulable;
            }
        }
        public void Schedule()
        {
            FireScheduleStarted();
            lock(this){
                this.Lock();
                SchedulerImpl();
                this.Unlock();
            }
            FireScheduleCompleted();
            FireStateChanged();
        }
        protected void SchedulerImpl()
        {
            _name2res.Clear();
            _id2task.Clear();
            foreach (ScheduleResource t in _resources)
            {
                _name2res[t.Name] = t;
            }
            LinkedListNode<T> node = _tasks.First;
            while (node != null)
            {
                //if (node.Value.Status == Status.NotScheduled)
                _id2task[node.Value.ID] = node.Value;
                node = node.Next;
            }
            foreach (ScheduleTask t in _tasks)
            {
                if (t.Status == Status.NotScheduled || t.Status == Status.Unscheduled)
                {
                    Boolean canSchedule = true;
                    if (t.Activities == null || t.Activities.Length == 0)
                        canSchedule = false;
                    else
                        foreach (ScheduleActivity act in t.Activities)
                        {
                            foreach (string res in act.Resources.Keys)
                            {
                                if (!_name2res.ContainsKey(res))
                                {
                                    canSchedule = false;
                                }
                                else if (_name2res[res].AvailableCount < act.Resources[res])
                                    canSchedule = false;
                            }
                        }
                    if (!canSchedule)
                        t.Status = Status.NotSchedulable;
                }
            }
            node = _tasks.First;
            while (node != null)
            {
                foreach (int key in node.Value.Relations.Keys)
                {
                    TaskRelation rel = node.Value.Relations[key];
                    ScheduleTask t = _id2task[key];
                    if (rel == TaskRelation.AfterTaskStart)
                    {
                        if (!node.Value.TasksRunAfterStart.Contains(t))
                        {
                            node.Value.TasksRunAfterStart.Add(t);
                        }
                    }
                    if (rel == TaskRelation.AfterTaskEnd)
                    {
                        if (!node.Value.TasksRunAfterEnd.Contains(t))
                        {
                            node.Value.TasksRunAfterEnd.Add(t);
                        }
                    }
                    if (rel == TaskRelation.BeforeRunStart)
                    {
                        if (!t.TasksRunAfterStart.Contains(node.Value))
                        {
                            t.TasksRunAfterStart.Add(node.Value);
                        }
                    }
                    if (rel == TaskRelation.BeforeRunEnd)
                    {
                        if (!t.TasksRunAfterEnd.Contains(node.Value))
                        {
                            t.TasksRunAfterEnd.Add(node.Value);
                        }
                    }

                }
                node = node.Next;
            }
            int count = 1;
            while (count > 0)
            {
                count = 0;
                node = _tasks.First;
                while (node != null)
                {
                    //Console.WriteLine("task {0} status is {1} and {2} activies", node.Value.ID, node.Value.Status, node.Value.Activities.Length);
                    if ((node.Value.Status == Status.NotScheduled || node.Value.Status == Status.Unscheduled)
                        && node.Value.Status != Status.NotSchedulable
                        )// && !_scheduledTasks.ContainsKey(node.Value.ID))
                    {
                        count++;
                        DoSchedule(node.Value);
                    }
                    node = node.Next;
                }
            }
        }

        public bool CheckActivityStart(ScheduleActivity next)
        {
            lock (this)
            {
                bool canRunNext = true;
                if (next != null && next.UnitReservations.Count > 0)
                {
                    for (int j = 0; j < next.UnitReservations.Count; j++)
                    {

                        //TODO互锁问题需要解决
                        LinkedListNode<UnitReservation> nur = next.UnitReservations[j].Previous;
                        if (nur != null)// && !currentUnit.ContainsKey(nur))
                        {
                            ScheduleActivity pRAct = nur.Value.Activity;
                            if (nur.Value.Activity.Status != Status.Processed && nur.Value.Activity.Status != Status.Cancelled && pRAct.WaitingForCompleted != true)
                            {
                                canRunNext = false;
                            }
                        }
                    }
                }
                return canRunNext;
            }
        }
        //event
        Boolean isScheduling=false;
        public bool Scheduling { get { return isScheduling; } }
        public event EventHandler<SchedulerStateChangeEventArg> StateChanged;
        public event EventHandler<SchedulerStateChangeEventArg> TaskScheduleStarted;
        public event EventHandler<SchedulerStateChangeEventArg> TaskScheduleCompleted;
        void FireScheduleStarted()
        {
            isScheduling = true;
            if (TaskScheduleStarted != null)
                TaskScheduleStarted(this, new SchedulerStateChangeEventArg());
        }
        void FireScheduleCompleted()
        {
            isScheduling = false;
            if (TaskScheduleCompleted != null)
                TaskScheduleCompleted(this, new SchedulerStateChangeEventArg());
        }
        void FireStateChanged()
        {
            if (StateChanged != null)
            {
                        StateChanged(this, new SchedulerStateChangeEventArg());
            }
        }
        void MoveActivityLeft(ScheduleActivity act, int offset)
        {
            ScheduleActivity a = act.Next;
            int offset2 = int.MaxValue;
            foreach (LinkedListNode<UnitReservation> ur in a.UnitReservations)
            {
                if (ur.Previous != null)
                {
                    int d = ur.Value.Activity.PlannedStart - (ur.Previous.Value.Activity.PlannedStart + ur.Previous.Value.Activity.PlannedDuration);
                    if (d < offset2)
                        offset2 = d;
                }
            }
            if (offset < offset2)
                offset2 = offset;
            if (offset2 <= 0)
                return;
            a.PlannedStart -= offset2;
            if (a.Next != null)
                MoveActivityLeft(a.Next, offset2);
            foreach (LinkedListNode<UnitReservation> ur in a.UnitReservations)
            {
                if (ur.Next != null)
                    MoveActivityLeft(ur.Next.Value.Activity, offset2);
            }
        }
        void MoveAllActivityleft(int offset)
        {
            LinkedListNode<T> node = _tasks.First;
            while (node != null)
            {
                ScheduleTask t = node.Value;
                if (t.Status != Status.Processed && t.Status != Status.Cancelled)
                {
                    for (int i = 0; i < t.Activities.Length; i++)
                    {
                        ScheduleActivity ac = t.Activities[i];
                        if (ac.Status == Status.Running)
                        {
                            ac.PlannedDuration -= offset;
                        }
                        else if (ac.Status != Status.Processed)
                        {
                            ac.PlannedStart -= offset;
                        }
                    }
                }
                node = node.Next;
            }
        }
        //>>>>>>>>>>>

        void DoMovePreActivity(ScheduleActivity act)
        {
            ScheduleActivity pre = act.Previous;
            while (pre != null)
            {
                if (pre.Status == Status.Processed )
                    return;
                ScheduleActivity next = pre.Next;
                int gap = next.PlannedStart - pre.PlannedStart - pre.PlannedDuration;
                if (gap <= 0)
                {
                    return;
                }
                bool canMove = true;
                foreach (LinkedListNode<UnitReservation> ur in pre.UnitReservations)
                {
                    if (ur.Next != null && ur.Next.Value.PlannedStart - ur.Value.PlannedStart - ur.Value.PlannedDuration < gap)
                    {
                        canMove = false;
                    }
                }
                if (pre.Status == Status.Running)
                {
                    pre.PlannedDuration += gap;
                    return;
                }
                if (canMove || next.Equals(act))
                {
                    pre.PlannedStart += gap;
                }
                else
                {
                    bool canStretch = true;
                    foreach (LinkedListNode<UnitReservation> ur in pre.Next.UnitReservations)
                    {
                        if (ur.Previous != null && ur.Value.PlannedStart - ur.Previous.Value.PlannedStart - ur.Previous.Value.PlannedDuration < gap)
                            canStretch = false;
                    }
                    if (canStretch)
                    {
                        next.PlannedStart -= gap;
                        next.PlannedDuration += gap;

                    }
                }
                pre = pre.Previous;
            }
        }
        void DoMoveActibityRight(ScheduleActivity act, int offset, int level = 0)
        {
            int start = act.PlannedStart;
            int duration = act.PlannedDuration;
            level += 1;
            //处理后边的activity
            if (act.Status != Status.Scheduled && act.Status != Status.Running)
                return;
            if (act.Next != null)
            {
                ScheduleActivity s = act.Next;

                if (s != null)
                {
                    //nextActs[s] = true;
                    int offset2 = s.PlannedStart - start - duration;
                    int offset3 = s.PlannedDuration - s.Duration;  
                    if (offset3>0 && offset3>=offset)
                    {
                        s.PlannedStart += offset;
                        s.PlannedDuration -= offset;
                    }
                    else if (offset3 > 0 && offset3<offset)
                    {
                        s.PlannedStart += offset3;
                        s.PlannedDuration -= offset3;
                        DoMoveActibityRight(s, offset - offset3, level);
                    }
                    else if (offset3<=0 && offset2 < offset && offset-offset2>0)
                    {
                        DoMoveActibityRight(s, offset - offset2, level);
                    }
                }
            }
            if (act.UnitReservations != null)
            {
                for (int i = 0; i < act.UnitReservations.Count; i++)
                {
                    //Dictionary<ScheduleActivity, bool> nextActs = new Dictionary<ScheduleActivity, bool>();
                    if (act.UnitReservations[i].Next != null)
                    {
                        ScheduleActivity s = act.UnitReservations[i].Next.Value.Activity;
                        //if (nextActs.ContainsKey(s))
                        if (s != null)
                        {
                            //nextActs[s] = true;

                            int offset2 = s.PlannedStart - start - duration;
                            int offset3 = s.PlannedDuration - s.Duration;
                            if (offset3 > 0 && offset3 >= offset)
                            {
                                s.PlannedStart += offset;
                                s.PlannedDuration -= offset;
                            }
                            else if (offset3 > 0 && offset3 < offset)
                            {
                                s.PlannedStart += offset3;
                                s.PlannedDuration -= offset3;
                                DoMoveActibityRight(s, offset - offset3, level);
                            }
                            else if (offset3 <= 0 && offset2 < offset && offset - offset2 > 0)
                            {
                                DoMoveActibityRight(s, offset - offset2, level);
                            }
                        }
                    }
                }
            }
            if (act.Index == act.Task.ActivityCount - 1)
            {
                LinkedListNode<T> node = _tasks.First;
                while (node != null)
                {
                    if (node.Value.TasksRunAfterEnd.Contains(act.Task))
                    {
                        ScheduleActivity s = node.Value.Activities[0];
                        if (s != null)
                        {
                            //nextActs[s] = true;
                            int offset2 = s.PlannedStart - start - duration;
                            if (offset2 < offset)
                            {
                                DoMoveActibityRight(s, offset - offset2, level);
                            }

                        }
                    }
                    node = node.Next;
                }
            }

            if (level != 1)
            {
                act.PlannedStart += offset;
                //DoMovePreActivity(act);
            }
            else
            {
                if (act.Status == Status.Scheduled)
                {
                    act.PlannedStart += offset;
                }
                else
                {
                    act.PlannedDuration += offset;
                }
            }
        }
        void MoveActivityRight(ScheduleActivity act, int offset)
        {
            DoMoveActibityRight(act, offset);
            int count = 0;
            while ((count = DoMoveGap()) > 0)
            {
            }
            DoMoveGapLeft();
        }
        void DoMoveGapLeft()
        {
            //前移有空隙的Activity
            LinkedListNode<T> node = _tasks.First;
            while (node != null)
            {
                ScheduleTask t = node.Value;
                if (!(t.Status == Status.Scheduled || t.Status == Status.Running))
                {
                    node = node.Next;
                    continue;
                }
                for (int i = t.Activities.Length - 1; i >=1 ; i--)
                {
                    ScheduleActivity aNext = t.Activities[i];
                    if (aNext.Status == Status.Processed || aNext.Status==Status.Cancelled || aNext.Status == Status.Running)
                    {
                        break;
                    }
                    int distance = aNext.PlannedStart - aNext.Previous.PlannedStart - aNext.Previous.PlannedDuration;
                    if (distance <= 0)
                        continue;
                    //前边activity向左延伸
                    bool canStretch = true;
                    foreach (LinkedListNode<UnitReservation> ur in aNext.UnitReservations)
                    {
                        if (ur.Previous != null)
                        {
                            int d = ur.Value.PlannedStart - ur.Previous.Value.PlannedStart - ur.Previous.Value.PlannedDuration;
                            if (d < distance)
                                canStretch = false;
                        }
                    }
                    if (canStretch)
                    {
                        aNext.PlannedStart -= distance;
                        aNext.PlannedDuration += distance;
                    }
                    else
                    {
                        MoveActivityRight(aNext.Previous, distance);
                    }
                }
                node = node.Next;
            }
        }
        int DoMoveGap()
        {
            //前移有空隙的Activity
            LinkedListNode<T> node = _tasks.Last;
            int movedCount = 0;

            while (node != null)
            {
                ScheduleTask t = node.Value;
                if (!(t.Status == Status.Scheduled || t.Status == Status.Running))
                {
                    node = node.Previous;
                    continue;
                }
                for (int i = t.Activities.Length - 2; i >= 0; i--)
                {
                    ScheduleActivity a = t.Activities[i];
                    ScheduleActivity aNext = t.Activities[i + 1];
                    bool canMove = true;
                    if (a.Status == Status.Processed || a.Status==Status.Cancelled)
                    {
                        break;
                    }
                    int distance = aNext.PlannedStart - a.PlannedStart - a.PlannedDuration;
                    if (distance <= 0)
                        continue;
                    foreach (LinkedListNode<UnitReservation> ur in a.UnitReservations)
                    {
                        if (ur.Next != null)
                        {
                            int d = ur.Next.Value.PlannedStart - ur.Value.PlannedDuration - ur.Value.PlannedStart;
                            if (d < distance)
                                distance = d;
                        }
                    }
                    if (distance <= 0)
                        continue;
                    if (a.Status == Status.Running)
                    {
                            a.PlannedDuration += distance;
                            movedCount++;
                    }
                    else if (canMove)
                    {
                        a.PlannedStart += distance;
                        movedCount++;
                    }
                }
                node = node.Previous;
            }
            return movedCount;
        }
        bool CheckConflict(string desc)
        {
            return false;
            bool ok = true;
            foreach (ScheduleResource res in _resources)
            {
                for (int i = 0; i < res.Count; i++)
                {
                    UnitReservations ur = res.GetReservationsForUnit(i);
                    LinkedListNode<UnitReservation> node = ur.First;
                    while (node != null)
                    {
                        if (node.Next != null)
                        {
                            if(ScheduleResource.IsIntersct2(node.Value.PlannedStart, node.Value.PlannedDuration+node.Value.PlannedStart,
                                node.Next.Value.PlannedStart, node.Next.Value.PlannedStart+node.Next.Value.PlannedDuration)){
                                    ScheduleActivity a = node.Value.Activity;
                                    ScheduleActivity aNext = node.Next.Value.Activity;
                                    MessageBox.Show(string.Format("activity {0}/{1} {4}-{5} and {2}/{3} {6}-{7} conflict",
                                        a.Name, a.Task.ID, aNext.Name, aNext.Task.ID, a.PlannedStart, a.PlannedDuration,
                                        aNext.PlannedStart, aNext.PlannedDuration),desc);
                                    ok = false;
                            }
                        }
                        node = node.Next;
                    }
                }
            }
            return !ok;
        }

        public void Lock()
        {
            Monitor.Enter(_tasks);
        }
        public void Unlock()
        {
            Monitor.Exit(_tasks);
        }
    }
}