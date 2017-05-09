using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;

namespace Scheduler
{
    public class SchedulerEngine : Scheduler<InternalTask, ITask>, ISchedulerEngine
    {
    }
}
