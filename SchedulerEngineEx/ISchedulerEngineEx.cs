using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scheduler;

namespace Scheduler
{
    public interface ISchedulerEngineEx : IRuntimeScheduler
    {
        int Activate(ITaskEx task);
        int Activate(ITaskEx task, Dictionary<int, TaskRelation> relations);
    }
}
