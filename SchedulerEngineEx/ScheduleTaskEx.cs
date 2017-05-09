using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scheduler;
using System.Threading;
using System.Drawing;

namespace Scheduler
{
    public class ScheduleTaskEx : ScheduleTask, ITaskControl
    {
        internal class ProxyTaskControl : ITaskControl
        {
            ITaskControl t;
            public ProxyTaskControl(ITaskControl t)
            {
                this.t = t;
            }

            public void StartTask(string name)
            {
                t.StartTask(name);
            }

            public bool Activity(string name, int duration, string[] resources, int[] count, Color color, int maxPlanDuration = 0)
            {
                return t.Activity(name, duration, resources, count, color, maxPlanDuration);
            }

            public void EndTask()
            {
                t.EndTask();
            }

            public int[] GetResources(string name)
            {
                return t.GetResources(name);
            }

            public string GetActivityName()
            {
                return t.GetActivityName();
            }

            public int GetActivityDuration()
            {
                return t.GetActivityDuration();
            }

            public int GetTaskId()
            {
                return t.GetTaskId();
            }


            public void CancelTask()
            {
                t.CancelTask();
            }
        }
        //event

        ITaskEx baseTask;
        public ScheduleActivity CurrentActivity { get; set; }
        public bool ScheduleMode { get; set; }
        public override void SetTask(object task)
        {
            baseTask = task as ITaskEx;
            baseTask.SetTaskControl(new ProxyTaskControl(this));
            ScheduleMode = true;
            baseTask.Run();
        }
        public override void Execute(object scheduler)
        {
            Scheduler = scheduler as SchedulerEngineEx;
            ScheduleMode = false;
            Status = Status.Running;
            try
            {
                baseTask.Run();
            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e.Message);
                Console.Out.WriteLine(e.StackTrace);
                Console.Out.Flush();
                CancelTask(CurrentActivity);
            }
        }

        public List<ScheduleActivity> embedActivities = new List<ScheduleActivity>();
        public void CancelTask()
        {
            CancelTask(CurrentActivity);
        }
        public void WaitForStart()
        {
            ScheduleTask t = this;
            while (true)
            {
                ScheduleActivity ac = t.Activities[0];
                bool canStart = true;
                foreach (LinkedListNode<UnitReservation> ur in ac.UnitReservations)
                {
                    if (ur.Previous != null &&
                        (ur.Previous.Value.Activity.Status != Status.Cancelled && ur.Previous.Value.Activity.Status != Status.Processed && ur.Previous.Value.Activity.WaitingForCompleted != true))
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
                if (Scheduler.CurrentExecuteTime < t.Activities[0].PlannedStart || Scheduler.IsRunning == false)
                {
                    canStart = false;
                }
                if (canStart)
                    break;
                else
                    Thread.Sleep(600);
            }
            Status = Status.Running;
            FireTaskStarted(this, new TaskEventArg(this));
        }
        public void WaitForComplete()
        {
            if (this.Activities.Length > 0)
            {
                ScheduleActivity act = this.Activities[this.Activities.Length - 1];
                if(act.Status==Status.Running)
                    FreeResource(act);
                act.WaitingForCompleted = true;
                act.Status = Status.Processed;
                act.ActualDuration = Scheduler.CurrentExecuteTime - act.ActualStart;
                FireActivityCompleted(this, new ActivityEventArg(act));
            }
            Status = Status.Processed;
            FireTaskCompleted(this, new TaskEventArg(this));
        }
        public void WaitForActivityStart(ScheduleActivity act)
        {
            if (act.Previous != null)
            {
                FreeResource(act.Previous);
                act.Previous.ActualDuration = Scheduler.CurrentExecuteTime - act.Previous.ActualStart;
                act.Previous.WaitingForCompleted = true;
            }
            while (true)
            {
                if (Scheduler.CheckActivityStart(act))
                {
                    break;
                }
                else
                    Thread.Sleep(100);
            }
            if (act.Previous != null)
            {
                act.Previous.Status = Status.Processed;
                FireActivityCompleted(this, new ActivityEventArg(act.Previous));
                Thread.Sleep(100);
            }
            act.Status = Status.Running;
            currentActivity = act;
            act.ActualStart = Scheduler.CurrentExecuteTime;
            LockResource(act);
            FireActivityStarted(this, new ActivityEventArg(act));
        }

        public void StartTask(string name)
        {
            if (ScheduleMode)
            {
                Name = name;
            }
            else
            {
                WaitForStart();
            }
        }

        public bool Activity(string name, int duration, string[] resources, int[] count, Color color, int maxPlanDuration = 0)
        {
            if (ScheduleMode)
            {
                ScheduleActivity act = new ScheduleActivity();
                act.Name = name;
                act.Duration = duration;
                act.MaxPlannedDuration = maxPlanDuration;
                act.Color = color;
                for (int i = 0; resources != null && i < resources.Length; i++)
                {
                    act.Resources[resources[i]] = count[i];
                }
                embedActivities.Add(act);
                return false;
            }
            else
            {
                if (Status == Status.Cancelled)
                    return false;
                if (CurrentActivity == null)
                    CurrentActivity = Activities[0];
                else
                    CurrentActivity = CurrentActivity.Next;
                WaitForActivityStart(CurrentActivity);
                return true;
            }
        }

        public void EndTask()
        {
            if (ScheduleMode)
            {
                Activities = embedActivities.ToArray();
            }
            else
            {
                if (Status == Status.Cancelled)
                    return;
                WaitForComplete();
            }
        }

        public int[] GetResources(string name)
        {
            return CurrentActivity.Reservations[name];
        }
        public string GetActivityName()
        {
            return CurrentActivity.Name;
        }
        public int GetActivityDuration()
        {
            if (CurrentActivity.PlannedDuration > 0)
                return CurrentActivity.PlannedDuration;
            else
                return 0;
        }

        public int GetTaskId()
        {
            return ID;
        }
    }
}
