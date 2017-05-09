using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scheduler
{
    public class RuntimeActivity : IRuntimeActivity
    {
        ScheduleActivity _activity;
        public RuntimeActivity(ScheduleActivity act)
        {
            _activity = act;
        }

        public string Name
        {
            get { return _activity.Name; }
        }

        public System.Drawing.Color Color
        {
            get { return _activity.Color; }
        }

        public Guid ID
        {
            get { return _activity.ID; }
        }

        public int Duration
        {
            get { return _activity.Duration; }
        }

        public int MaxPlannedDuration
        {
            get { return _activity.MaxPlannedDuration; }
        }

        public ResourceRequired Resources
        {
            get { return _activity.Resources; }
        }

        public int PlannedStart
        {
            get { return _activity.PlannedStart; }
        }

        public int PlannedDuration
        {
            get { return _activity.PlannedDuration; }
        }

        public Dictionary<string, int[]> Reservations
        {
            get { return _activity.Reservations; }
        }

        public Status Status
        {
            get { return _activity.Status; }
        }
    }
}
