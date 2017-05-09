using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scheduler
{
    public class Resource : IResource
    {
        public string Name
        {
            get;
            set;
        }

        public int Count
        {
            get;
            set;
        }

        public System.Drawing.Color Color
        {
            get;
            set;
        }
    }
}
