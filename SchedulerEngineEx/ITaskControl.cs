using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Scheduler
{
    public delegate void TaskStarter();
    public interface ITaskControl
    {
        void StartTask(string name);
        void Activity(string name, int duration, string[] resources, int[] count, Color color, int maxPlanDuration, TaskStarter func);
        void EndTask();

        void CancelTask();
        int[] GetResources(string name);
        string GetActivityName();
        int GetActivityDuration();
        int GetTaskId();
    }
}
