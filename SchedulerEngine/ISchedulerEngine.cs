using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Scheduler
{
    interface ISchedulerEngine : IRuntimeScheduler
    {
        int Activate(ITask t);
        int Activate(ITask t, Dictionary<int, TaskRelation> relations);
    }
}
