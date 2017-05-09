using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scheduler
{
    public class Task : ITask
    {
        public string Name
        {
            get;
            set;
        }
        public IActivity[] Schedule()
        {
            return Activities;
        }
        public IActivity[] Activities
        {
            get;
            set;
        }

        public Dictionary<int, TaskRelation> Relations
        {
            get { return rel; }
        }
        Dictionary<int, TaskRelation> rel = new Dictionary<int, TaskRelation>();
    }
}
