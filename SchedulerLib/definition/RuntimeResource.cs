using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scheduler
{
    public class RuntimeResource : IRuntimeResource
    {
        ScheduleResource _resource;
        public RuntimeResource(ScheduleResource r){
            _resource = r;
        }
        public int AvailableCount
        {
            get { return _resource.AvailableCount; }
        }

        public void DisableUnit(int unit)
        {
            _resource.DisableUnit(unit);
        }

        public void EnableUnit(int unit)
        {
            _resource.EnableUnit(unit);
        }

        public bool IsUnitEnabled(int unit)
        {
            return _resource.IsUnitEnabled(unit);
        }

        public string Name
        {
            get { return _resource.Name; }
        }

        public int Count
        {
            get { return _resource.Count; }
        }

        public System.Drawing.Color Color
        {
            get { return _resource.Color; }
        }
    }
}
