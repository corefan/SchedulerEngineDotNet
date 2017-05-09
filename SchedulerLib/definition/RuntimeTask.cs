using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scheduler
{
    public class RuntimeTask : IRuntimeTask
    {
        ScheduleTask _task;

        public RuntimeTask(ScheduleTask t)
        {
            _task = t;
        }

        List<IRuntimeTask> _TasksRunAfterStart = new List<IRuntimeTask>();
        List<IRuntimeTask> _TasksRunAfterEnd = new List<IRuntimeTask>();

        public List<IRuntimeTask> TasksRunAfterStart
        {
            get
            {
                List<IRuntimeTask> task = new List<IRuntimeTask>();
                for (int i = 0; i < _task.TasksRunAfterStart.Count; i++)
                    task.Add(_task.TasksRunAfterStart[i].RuntimeTask);
                return task;
            }
        }
        public List<IRuntimeTask> TasksRunAfterEnd
        {
            get
            {
                List<IRuntimeTask> task = new List<IRuntimeTask>();
                for (int i = 0; i < _task.TasksRunAfterEnd.Count; i++)
                    task.Add(_task.TasksRunAfterEnd[i].RuntimeTask);
                return task;
            }
        }

        public string Name
        {
            get { return _task.Name; }
        }

        public Status Status
        {
            get { return _task.Status; }
        }

        public int ID
        {
            get
            {
                return _task.ID;
            }
        }

        public Dictionary<int, TaskRelation> Relations
        {
            get { return _task.Relations; }
        }

        public int ActivityCount
        {
            get { return _task.ActivityCount; }
        }

        public IRuntimeActivity GetActivity(int index)
        {
            return _task.GetActivity(index);
        }


        public IRuntimeActivity[] Activities
        {
            get { return _task.RunTimeActivities; }
        }

        public int PlannedStart
        {
            get { return _task.PlannedStart; }
        }
        public int Duration
        {
            get { return _task.Duration; }
        }
        public int PlannedDuration
        {
            get { return _task.PlannedDuration; }
        }

        public double ActualStart
        {
            get { return _task.ActualStart; }
        }
        public double ActualDuration
        {
            get { return _task.ActualDuration; }
        }
    }
}
