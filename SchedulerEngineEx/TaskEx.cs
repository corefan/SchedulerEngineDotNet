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
    }
}
