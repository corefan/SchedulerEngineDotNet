using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scheduler
{
    public class SchedulerSetting
    {
        private SchedulerSetting()
        {
        }
        static SchedulerSetting instance = new SchedulerSetting();
        public static SchedulerSetting Instance
        {
            get
            {
                return instance;
            }
        }
        float enlarge = 1.1f;
        public float MaxPlantimeEnlarge
        {
            get
            {
                return enlarge;
            }
            set
            {
                if (value >= 1)
                    enlarge = value;
            }
        }
        float decrease = 0;
        public float DefaultDecreaseRate
        {
            get
            {
                return decrease;
            }
            set { decrease = value; }
        }
    }
}
