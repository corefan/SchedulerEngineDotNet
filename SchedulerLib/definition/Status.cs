using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scheduler
{

    public enum Status
    {
        NotScheduled = 1, // The task is not scheduled.
        NotSchedulable = 2, // The task is not schedulable.
        Scheduled = 3, // The task is scheduled.
        Running = 4,  // The task is running.
        Cancelling = 5, // The task is cancelling.
        Cancelled = 6, // The task is cancelled.
        Processed = 8, // The task is processed.
        Unscheduled = 9 // The task is unscheduled.
    }
}
