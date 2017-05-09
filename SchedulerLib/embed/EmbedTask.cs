using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Scheduler
{
    public class EmbedTask : ScheduleTask
    {
        internal class EmbedActivityRunContext : IActivityRunContext
        {
            public EmbedActivityRunContext()
            {
            }
            string _name;
            Dictionary<string, int[]> _resource;
            int _duration;
            public void SetData(ScheduleActivity ac)
            {
                _name = ac.Name;
                _resource = ac.Reservations;
                _duration = ac.PlannedDuration;
            }
            public int[] GetResources(string name)
            {
                return _resource[name];
            }

            public string GetActivityName()
            {
                return _name;
            }

            public int GetActivityDuration()
            {
                return _duration;
            }
        }
        //event
        public override void SetTask(object t)
        {
            ITask task = t as ITask;
            if (task == null)
                throw new ArgumentException("object type wrong");
            Name = task.Name;
            IActivity[] actFromTask = task.Schedule();
            ScheduleActivity[] acts = new ScheduleActivity[actFromTask.Length];
            for (int i = 0; i < acts.Length; i++)
            {
                acts[i] = new ScheduleActivity(actFromTask[i]);
                acts[i].Index = i;
                acts[i].Task = this;
            }
            Activities = acts;
            _task = task;
        }

        ITask _task;

        public ITask Task { get { return _task; } }
        public override void Execute(object scheduler)
        {
            _scheduler = scheduler as EmbedScheduler;

            Status = Status.Running;
            //new Thread(new ThreadStart(Run)).Start();
            Run();
        }
        EmbedScheduler _scheduler;


        void Run()
        {
            Console.WriteLine("task {0} started===================+++++++++++++++++++++++", this.ID);
            FireTaskStarted(this, new TaskEventArg(this));
            EmbedActivityRunContext context = new EmbedActivityRunContext();
            for (int i = 0; i < this.Activities.Length; i++)
            {
                ScheduleActivity ac = this.Activities[i];
                currentActivity = ac;
                ac.ActualStart = _scheduler.CurrentExecuteTime;
                ac.Status = Status.Running;
                FireActivityStarted(this, new ActivityEventArg(ac));
                Console.WriteLine("task {0} activity {1} started", this.ID, ac.Name);
                LockResource(ac);
                context.SetData(ac);
                try{
                    ac.Execute(context);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                }
                ac.ActualDuration = _scheduler.CurrentExecuteTime - ac.ActualStart;
                FreeResource(ac);
                //ac.Status = Status.Processed;
                ac.WaitingForCompleted = true;
                while (true)
                {
                    //检查下一个Activity是否可以开始,计算后便所有Activity的开始和结束时间
                    ScheduleActivity next = ac.Next;
                    bool canRunNext = CheckActivityStart(next);
                    if (canRunNext)
                    {
                        //是否提前结束
                        ac.Status = Status.Processed;
                        Console.WriteLine("task {0} activity {1} completed++", this.ID, ac.Name);
                        FireActivityCompleted(this, new ActivityEventArg(ac));
                        currentActivity = null;
                        break;
                    }
                    Thread.Sleep(100);

                }
            }
            Status = Status.Processed;
            Console.WriteLine("task {0}  completed+++++++++++++++++++++++", this.ID);
            FireTaskCompleted(this, new TaskEventArg(this));
        }
    }
}
