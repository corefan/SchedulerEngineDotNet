using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;

namespace Scheduler
{
    public class Activity : IActivity
    {
        public Activity()
        {
        }
        static Random r = new Random();
        public void Execute(IActivityRunContext context)
        {
           Thread.Sleep(r.Next((int)(Duration*0.8),(int)(Duration*1.6))*1000);
        }
        public string Name
        {
            get;
            set;
        }

        public Color Color
        {
            get;
            set;
        }

        public Guid ID
        {
            get { return id; }
        }
        Guid id = Guid.NewGuid();
        public int Duration
        {
            get;
            set;
        }

        public int MaxPlannedDuration
        {
            get;
            set;
        }

        public ResourceRequired Resources
        {
            get { return res; }
        }
        ResourceRequired res = new ResourceRequired();


        public IActivity Cancel
        {
            get;
            set;
        }
    }
}
