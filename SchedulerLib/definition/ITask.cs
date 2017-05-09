using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scheduler
{
    public delegate void TaskFunctionHandler();
    //for programming or application
    public interface IRuntimeTask
    {
        string Name { get; }
        Status Status { get; }
        int ID { get;}

        int PlannedStart
        {
            get;
        }
        int Duration
        {
            get;
        }
        int PlannedDuration
        {
            get;
        }

        double ActualStart
        {
            get;
        }
        double ActualDuration
        {
            get;
        }

        Dictionary<int, TaskRelation> Relations { get; }
        int ActivityCount { get; }
        IRuntimeActivity GetActivity(int index);
        IRuntimeActivity[] Activities { get; }

        List<IRuntimeTask> TasksRunAfterStart { get ; } 
        List<IRuntimeTask> TasksRunAfterEnd { get; }
    }
    //for scheduler
    /*
    public interface IScheduleTask2
    {
        ITask Task { get;}
        string Name { get; }

        Status Status { get; set; }
        int ID { get; set; }
        //IScheduleActivity[] ScheduleActivities { get; }
        Dictionary<int, TaskRelation> Relations { get; }
    }
     */
}
