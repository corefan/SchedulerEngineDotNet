using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Scheduler
{
    public interface ITaskControl
    {
        void StartTask(string name);
        bool Activity(string name, int duration, string[] resources, int[] count, Color color, int maxPlanDuration = 0);
        void EndTask();

        void CancelTask();
        int[] GetResources(string name);
        string GetActivityName();
        int GetActivityDuration();
        int GetTaskId();
    }
}
