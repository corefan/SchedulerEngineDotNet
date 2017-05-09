using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Scheduler
{
    public interface IActivity
    {
        string Name { get; }
        Color Color { get; }
        Guid ID { get; }
        int Duration { get; }
        int MaxPlannedDuration { get; }
        ResourceRequired Resources { get; }
        void Execute(IActivityRunContext context);
        IActivity Cancel { get; }
    }
    public interface IActivityRunContext
    {
        int[] GetResources(string name);
        string GetActivityName();
        int GetActivityDuration();
    }
    public interface IRuntimeActivity
    {
        string Name { get; }
        Color Color { get; }
        Guid ID { get; }
        int Duration { get; }
        int MaxPlannedDuration { get; }
        ResourceRequired Resources { get; }
        int PlannedStart { get;}
        int PlannedDuration { get; }
        Dictionary<string, int[]> Reservations { get; }
        Status Status { get; }
    }
    public class ResourceRequired : Dictionary<string, int>
    {
    }
}
