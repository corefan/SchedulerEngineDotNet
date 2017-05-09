using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Scheduler
{

    public class ScheduleTask
    {
        public ScheduleTask()
        {
            _runtimeTask = new RuntimeTask(this);
        }

        public string Name { get; set; }
        public Status Status { get; set; }
        public int ID { get; set; }

        public int ActivityCount { get { return _acs.Length; } }
        public ScheduleActivity[] Activities
        {
            get { return _acs; }
            set
            {
                _acs = value;
                _rActivities = new RuntimeActivity[value.Length];
                for (int i = 0; i < value.Length; i++)
                {
                    //if (i == 0)
                    //    node = ScheduleActivities.AddFirst(value[i]);
                    //else
                    //    node = ScheduleActivities.AddAfter(node, value[i]);
                    value[i].Task = this;
                    if (i > 0)
                        value[i].Previous = value[i - 1];
                    if (i < value.Length - 1)
                        value[i].Next = value[i + 1];
                    _rActivities[i] = new RuntimeActivity(_acs[i]);

                }
            }
        }
        //public TaskActivities ScheduleActivities { get { return _sacs; } }
        public Dictionary<int, TaskRelation> Relations { get { return _relations; } }

        Dictionary<int, TaskRelation> _relations = new Dictionary<int, TaskRelation>();
        List<ScheduleTask> _tasksRunAfterStart = new List<ScheduleTask>();
        List<ScheduleTask> _tasksRunAfterEnd = new List<ScheduleTask>();
        public List<ScheduleTask> TasksRunAfterStart { get { return _tasksRunAfterStart; } }
        public List<ScheduleTask> TasksRunAfterEnd { get { return _tasksRunAfterEnd; } }

        ScheduleActivity[] _acs;
        //IActivity[] _baseAcs;
        //TaskActivities _sacs = new TaskActivities();

        public int PlannedStart
        {
            get
            {
                if (Activities == null || Activities.Length == 0) return 0;
                return Activities[0].PlannedStart;
            }
        }
        public int Duration
        {
            get
            {
                int d = 0;
                if (Activities != null)
                {
                    for (int i = 0; i < Activities.Length; i++)
                        d += Activities[i].Duration;
                }
                return d;
            }
        }
        public int PlannedDuration
        {
            get
            {
                int d = 0;
                if (Activities != null)
                {
                    for (int i = 0; i < Activities.Length; i++)
                        if(Activities[i].Status!=Status.Cancelled)
                            d += Activities[i].PlannedDuration;
                }
                return d;
            }
        }

        public double ActualStart
        {
            get
            {
                return this.Activities[0].ActualStart;
            }
        }
        public double ActualDuration
        {
            get
            {
                double d = 0;
                for (int i = 0; i < this.Activities.Length; i++)
                {
                    if (this.Activities[i].ActualDuration == -1)
                    {
                        return -1;
                    }
                    d += this.Activities[i].ActualDuration;
                }
                return d;
            }
        }
        
        public RuntimeActivity GetActivity(int index)
        {
            return _rActivities[index];
        }
        RuntimeActivity[] _rActivities;
        public RuntimeActivity[] RunTimeActivities
        {
            get
            {
                return _rActivities;
            }
        }

        public RuntimeTask RuntimeTask { get { return _runtimeTask; } }

        public IRuntimeScheduler Scheduler { get; set; }

        RuntimeTask _runtimeTask;

        //virtual function for override
        public virtual void SetTask(object task) { }

        public virtual void Execute(object scheduler) { }


        protected ScheduleActivity currentActivity = null;
        public ScheduleActivity CurrentRunningActivity
        {
            get { return currentActivity; }
        }
        //event

        public event SchedulerEventHandler<TaskEventArg> TaskStarted;
        public event SchedulerEventHandler<TaskEventArg> TaskCompleted;
        public event SchedulerEventHandler<ActivityEventArg> ActivityStarted;
        public event SchedulerEventHandler<ActivityEventArg> ActivityCancelled;
        public event SchedulerEventHandler<ActivityEventArg> ActivityCompleted;
        public event SchedulerEventHandler<TaskEventArg> TaskCancelled;
        protected void FireTaskStarted(object sender, TaskEventArg e)
        {
            if (TaskStarted != null)
                TaskStarted(sender, e);
        }
        protected void FireTaskCompleted(object sender, TaskEventArg e)
        {
            if (TaskCompleted != null)
                TaskCompleted(sender, e);
        }
        protected void FireTaskCancelled(object sender, TaskEventArg e)
        {
            if (TaskCancelled != null)
                TaskCancelled(sender, e);
        }
        protected void FireActivityStarted(object sender, ActivityEventArg e)
        {
            if (ActivityStarted != null)
                ActivityStarted(sender, e);
        }
        protected void FireActivityCompleted(object sender, ActivityEventArg e)
        {
            if (ActivityCompleted != null)
                ActivityCompleted(sender, e);
        }
        protected void FireActivityCancelled(object sender, ActivityEventArg e)
        {
            if (ActivityCancelled != null)
                ActivityCancelled(sender, e);
        }

        public void CancelTask(ScheduleActivity a)
        {
            if (a != null)
            {
                a.ActualDuration = Scheduler.Current - a.ActualStart;
                FreeResource(a);
                //a.Status = Status.Cancelled;
                Status = Status.Cancelled;
                FireActivityCancelled(this, new ActivityEventArg(a));
            }
            //Status = Status.Cancelled;
            FireTaskCancelled(this, new TaskEventArg(this));
        }

        protected void LockResource(ScheduleActivity activity)
        {
            Dictionary<ScheduleResource, List<int>> res = new Dictionary<ScheduleResource, List<int>>();
            for(int i=0;i<activity.UnitReservations.Count;i++){
                LinkedListNode<UnitReservation> node = activity.UnitReservations[i];
                if (!res.ContainsKey(node.Value.Resource))
                    res[node.Value.Resource] = new List<int>();
                res[node.Value.Resource].Add(node.Value.Uint);
            }
            foreach (ScheduleResource r in res.Keys)
            {
                r.LockUnits(res[r].ToArray(), this, activity);
            }
        }
        protected void FreeResource(ScheduleActivity activity)
        {
            Dictionary<ScheduleResource, List<int>> res = new Dictionary<ScheduleResource, List<int>>();
            for (int i = 0; i < activity.UnitReservations.Count; i++)
            {
                LinkedListNode<UnitReservation> node = activity.UnitReservations[i];
                if (!res.ContainsKey(node.Value.Resource))
                    res[node.Value.Resource] = new List<int>();
                res[node.Value.Resource].Add(node.Value.Uint);
            }
            foreach (ScheduleResource r in res.Keys)
            {
                r.FreeUnits(res[r].ToArray(), this, activity);
            }
        }

    }
}
