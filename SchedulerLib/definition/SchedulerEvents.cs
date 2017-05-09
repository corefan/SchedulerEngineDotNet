using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scheduler
{
    public class SchedulerStateChangeEventArg : EventArgs
    {
    }
    public class ActivityEventArg : EventArgs
    {
        public ActivityEventArg(ScheduleActivity activity)
        {
            Activity = activity;
        }
        public ScheduleActivity Activity { get; set; }
    }
    public class TaskEventArg
    {
        public TaskEventArg(ScheduleTask task)
        {
            Task = task;
        }
        public ScheduleTask Task { get; set; }
    }
    public class ResourceEventArgs
    {
        public ResourceEventArgs(string name, int unit, bool free, ScheduleTask task, ScheduleActivity activity)
        {
            _name = name;
            _unit = unit;
            _isFree = free;
        }
        bool _isFree = false;
        int _unit;
        string _name;
        public int Unit { get { return _unit; } }
        public string Name { get { return _name; } }
        public bool IsFree
        {
            get
            {
                return _isFree;
            }
        }
    }
    public delegate void SchedulerEventHandler<T>(object sender, T eventArgs);
}
