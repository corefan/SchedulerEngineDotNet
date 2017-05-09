using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Scheduler
{
    public interface IResource
    {
        string Name { get; }
        int Count { get; }
        Color Color { get; }
    }
    public interface IRuntimeResource : IResource
    {
        int AvailableCount { get; }
        void DisableUnit(int unit);
        void EnableUnit(int unit);
        bool IsUnitEnabled(int unit);
    }
    public class ResourceReservations : List<ResourceReservation>
    {
    }

    public class ResourceReservation
    {
        public ScheduleActivity Activity;
        public int[] Uint = new int[0];
        public int PlannedStart
        {
            get { return Activity.PlannedStart; }
        }
        public int PlannedDuration
        {
            get { return Activity.PlannedDuration; }
        }
    }
    public class UnitReservations : LinkedList<UnitReservation>
    {
        ScheduleResource _res;
        int _unit;
        public UnitReservations(ScheduleResource res, int unit)
        {
            _res = res;
            _unit = unit;
        }
        public ScheduleResource Resource
        {
            get { return _res; }
        }
        public int Unit
        {
            get { return _unit; }
        }
    }
    public class UnitReservation
    {
        public ScheduleActivity Activity;
        public ScheduleResource Resource;
        public int Uint = -1;
        public int PlannedStart
        {
            get { return Activity.PlannedStart; }
        }
        public int PlannedDuration
        {
            get { return Activity.PlannedDuration; }
        }
    }

}
