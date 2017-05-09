using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scheduler
{

    public interface ITask
    {
        string Name { get; }
        IActivity[] Schedule();
        /*Task相互关系
         * 
         */
        Dictionary<int, TaskRelation> Relations { get; }
    }
}
