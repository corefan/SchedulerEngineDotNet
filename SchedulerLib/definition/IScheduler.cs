using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scheduler
{
    public interface IScheduler<T>
    {
        void Register(IResource res);
        int Activate(T task);
        int Activate(T task, Dictionary<int, TaskRelation> relations);
        void Schedule();
        void Run();
    }
    public interface IRuntimeScheduler
    {
        IRuntimeTask GetTask(int id);
        IRuntimeResource GetResource(string name);
        IRuntimeResource[] Resources { get; }
        IRuntimeTask[] Tasks { get; }
        event EventHandler<SchedulerStateChangeEventArg> StateChanged;
        event EventHandler<SchedulerStateChangeEventArg> TaskScheduleCompleted;
        event EventHandler<SchedulerStateChangeEventArg> TaskScheduleStarted;
        int Current { get; }
        Nullable<DateTime> ExecuteStartTime { get; }
        void Schedule();
        void Run();
        void Register(IResource res);

        void Register(string name, int count, int color);
        bool IsRunning { get; }
        bool Scheduling { get; }
        bool CheckActivityStart(ScheduleActivity next);
        double CurrentExecuteTime { get; }


        void Lock();
        void Unlock();
    }
    public enum TaskRelation
    {
        AfterTaskStart=1,
        //目标task结束后启动
        AfterTaskEnd=2,
        //目标task开始后启动
        AfterRunStart=0,
        //目标task开始前启动
        BeforeRunStart=3,
        //目标task开始前结束
        BeforeRunEnd=4
    }
}
