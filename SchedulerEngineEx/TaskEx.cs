using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Scheduler
{
    public abstract class TaskEx : ITaskEx
    {
        ITaskControl t;
        public void SetTaskControl(ITaskControl t)
        {
            this.t = t;
        }

        public abstract void Run();
        public void StartTask(string name)
        {
            t.StartTask(name);
        }
        public void Activity(string name, int duration, string[] resources, int[] count, Color color, TaskStarter func)
        {
            t.Activity(name, duration, resources, count, color, 0, func);
        }
        public void Activity(string name, int duration, string[] resources, int[] count, Color color, int maxPlanDuration, TaskStarter func)
        {
            t.Activity(name, duration, resources, count, color, maxPlanDuration, func);
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
    }
}
