using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Scheduler
{
    public class ScheduleActivity
    {
        IActivity _baseActivity;
        public ScheduleActivity(IActivity activity)
        {
            Name = activity.Name;
            Color = activity.Color;
            Duration = activity.Duration;
            MaxPlannedDuration = activity.MaxPlannedDuration;
            id = activity.ID;
            _resources = activity.Resources;
            //Execute = activity.Execute;
            if (activity.Cancel != null)
                Cancel = new ScheduleActivity(activity.Cancel);
            _baseActivity = activity;
            ActualDuration =-1;
            ActualStart = -1;
        }
        public ScheduleActivity() { }
        public string Name { get; set; }
        public Color Color { get; set; }
        public int Duration { get; set; }
        public int PlannedStart { get; set; }
        public int PlannedDuration { get; set; }
        public int MaxPlannedDuration { get; set; }
        //public int NextFreeOffset { get; set; }
        public int NextAvailableTime { get; set; }
        public int Index { get; set; }

        public double ActualStart { get; set; }
        public double ActualDuration { get; set; }
        public bool ExecuteTimeExceedPlan { get; set; }

        public ResourceRequired Resources { get { return _resources; } }
        public Guid ID { get { return id; } }
        public Dictionary<string, int[]> Reservations { get { return _resv; } }
        public List<LinkedListNode<UnitReservation>> UnitReservations {get{return _unitReservations;}}
        public Status Status { get; set; }

        public void Execute(IActivityRunContext context)
        {
            _baseActivity.Execute(context);
        }
        public ScheduleActivity Cancel
        {
            get;
            set;
        }

        Guid id = Guid.NewGuid();
        ResourceRequired _resources = new ResourceRequired();
        List<LinkedListNode<UnitReservation>> _unitReservations = new List<LinkedListNode<UnitReservation>>();
        Dictionary<string, int[]> _resv = new Dictionary<string, int[]>();


        public ScheduleActivity Next { get; set; }
        public ScheduleActivity Previous { get; set; }
        public ScheduleTask Task { get; set; }


        //running control
        bool _waitForStart = false;
        public bool WaitingForCompleted
        {
            get;
            set;
        }

    }
}
