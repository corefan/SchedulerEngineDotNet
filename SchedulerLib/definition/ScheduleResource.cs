using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Scheduler
{
    public class ScheduleResource 
    {
        public ScheduleResource(IResource res)
        {
            Name = res.Name;
            Count = res.Count;
            Color = res.Color;
        }
        public float DefaultEnlargetFactor = 1.1f;
        public bool EnableActivityEnlarge = true;
        public ScheduleResource() { }
        public string Name { get; set; }
        public int Count
        {
            get { return _count; }
            set {
                _unitEnabled = new bool[value];
                _unitReservations=new UnitReservations[value];
                _unitStatus = new bool[value];
                _count = value;
                for (int i = 0; i < value; i++){
                    _unitEnabled[i] = true;
                    _unitReservations[i]=new UnitReservations(this,i);
                    _unitStatus[i] = true;
                }
            }
        }
        public Color Color { get; set; }
        public ResourceReservations Reservations { get { return _reservation; } }

        public int AvailableCount
        {
            get
            {
                int u = 0;
                if (_count <= 0)
                    return 0;
                for (int i = 0; i < _count; i++)
                    if (_unitEnabled[i])
                        u++;
                return u;
            }
        }
        public void DisableUnit(int unit)
        {
            if (unit < 0 || unit > _count - 1)
                return;
            _unitEnabled[unit] = false;
        }
        public void EnableUnit(int unit)
        {
            if (unit < 0 || unit > _count - 1)
                return;
            _unitEnabled[unit] = true;
        }
        public bool IsUnitEnabled(int unit)
        {
            if (unit < 0 || unit > _count - 1)
                return false;
            return _unitEnabled[unit];
        }
        bool[] _unitEnabled;
        bool[] _unitStatus;
        int _count;
        ResourceReservations _reservation = new ResourceReservations();
        UnitReservations[] _unitReservations;

        public UnitReservations GetReservationsForUnit(int unit)
        {
            return _unitReservations[unit];
        }
        IRuntimeResource _runtimeReource;
        public IRuntimeResource RuntimeResource
        {
            get
            {
                if (_runtimeReource == null)
                    _runtimeReource = new RuntimeResource(this);
                return _runtimeReource;
            }
        }
        public int[] ReserveUnits(ScheduleActivity activity, int start, int duration, int[] unit)
        {
            int end = start + duration - 1;
            Dictionary<int, bool> used = new Dictionary<int, bool>();
            foreach (ResourceReservation u in Reservations)
            {

                //if ((start >= u.PlannedStart && start <= (u.PlannedStart + u.PlannedDuration - 1)) ||
                //    (end >= u.PlannedStart && end <= (u.PlannedStart + u.PlannedDuration - 1)) ||
                //    (u.PlannedStart >= start && u.PlannedStart <= end) || (u.PlannedStart + u.PlannedDuration - 1 >= start && u.PlannedStart + u.PlannedDuration - 1 <= end)
                //    )
                if (GetIntersetLength(start, end, u.PlannedStart, u.PlannedDuration + u.PlannedStart )>0)
                {
                    for (int i = 0; i < u.Uint.Length; i++)
                        used[u.Uint[i]] = true;
                }
            }
            bool canUsed = true;
            for (int i = 0; i < unit.Length; i++)
                if (used.ContainsKey(unit[i]) || !this.IsUnitEnabled(unit[i]))
                    canUsed = false;
            if (canUsed)
            {
                AddReservation(new ResourceReservation() { Activity = activity, Uint = unit });
                return unit;
            }
            return Reserve(activity,start,duration,unit.Length);
        }
        public static bool IsIntersct2(int start1, int end1, int start2, int end2)
        {
            int start = Math.Min(start1, start2);
            int end = Math.Max(end1, end2);
            if (end - start < (end1 - start1 + end2 - start2))
                return true;
            return false;
        }
        int GetIntersetLength(int start1, int end1, int start2, int end2)
        {
            //与前边actibity相交，不可以
            int start = Math.Min(start1, start2);
            int end = Math.Max(end1, end2);
            int r= (end1 - start1 + end2 - start2) - (end - start);
            if (r > 0){
                if(start2 > start1)
                return r;
            else 
                return end1;
            }
            return 0;
        }
        /*检查资源是否可用，如果可用返回资源Unit
         * 如果不可用，返回null，并且返回后边最近可用的时间偏移
         */
        public int[] CheckReservationForward(ScheduleActivity activity, int start, int duration, int count, out int planedDuration, out int NextFreeTimeOffset)
        {
            return CheckReservationInternal(activity, start, duration, count, out planedDuration, out NextFreeTimeOffset, false);
        }
        public int[] CheckReservation(ScheduleActivity activity, int pstart, int pduration, int count)
        {
            int start = pstart;
            int duration = pduration;
            Dictionary<ScheduleActivity, bool> preActsUseRes = new Dictionary<ScheduleActivity, bool>();
            //计算前后使用相同资源的Activity，合并一起计算
            {
                ScheduleActivity preAct = activity.Previous;
                while (preAct != null)
                {
                    if (preAct.Resources.ContainsKey(Name) && preAct.Resources[Name] == count)
                    {
                        preActsUseRes[preAct] = true;
                        start = preAct.PlannedStart;
                        duration += preAct.PlannedDuration;
                    }
                    else
                    {
                        break;
                    }
                    preAct = preAct.Previous;
                }
                preAct = activity.Next;
                while (preAct != null)
                {
                    if (preAct.Resources.ContainsKey(Name) && preAct.Resources[Name] == count)
                    {
                        duration += preAct.Duration;
                    }
                    else
                    {
                        break;
                    }
                    preAct = preAct.Next;
                }
            }
            int end = start + duration;
            int[] units = new int[count];
            for (int i = 0; i < units.Length; i++)
            {
                units[i] = -1;
            }
            int current = 0;
            //计算资源使用情况
            Dictionary<int, bool> used = new Dictionary<int, bool>();
            foreach (ResourceReservation u in Reservations)
            {
                //排除前边连续使用相同资源的Activity
                if (!preActsUseRes.ContainsKey(u.Activity))
                {
                    //if ((start >= u.PlannedStart && start <= (u.PlannedStart + u.PlannedDuration - 1)) ||
                    //    (end >= u.PlannedStart && end <= (u.PlannedStart + u.PlannedDuration - 1)) ||
                    //    (u.PlannedStart >= start && u.PlannedStart <= end) || (u.PlannedStart + u.PlannedDuration - 1 >= start && u.PlannedStart + u.PlannedDuration - 1 <= end))
                    if (GetIntersetLength(start, end, u.PlannedStart, u.PlannedDuration + u.PlannedStart) > 0)
                    {
                        for (int i = 0; i < u.Uint.Length; i++)
                            used[u.Uint[i]] = true;
                    }
                }
            }
            //计算前一个Activity资源，如果可能重复使用
            bool canUsePreActRes = false;
            if (activity.Previous != null && activity.Previous.Reservations.ContainsKey(Name))
            {
                canUsePreActRes = true;
                int[] us = activity.Previous.Reservations[Name];
                if (us != null && us.Length >= units.Length)
                {
                    for (int i = 0; i < us.Length; i++)
                    {
                        if (used.ContainsKey(us[i]) || !this.IsUnitEnabled(us[i]))
                            canUsePreActRes = false;
                    }
                    if (canUsePreActRes)
                    {
                        for (int i = 0; i < units.Length; i++)
                            units[i] = us[i];
                    }
                }
            }
            if (!canUsePreActRes)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    if (!used.ContainsKey(i) && this.IsUnitEnabled(i))
                    {
                        units[current] = i;
                        current++;
                        if (current == count)
                            break;
                    }
                }
                if (current == count)
                {
                    canUsePreActRes = true;
                }
            }
            if (canUsePreActRes)
                return units;
            return null;
        }
        private int[] CheckReservationInternal(ScheduleActivity activity, int pstart, int pduration, int count, out int planedDuration, out int NextFreeTimeOffset, bool onlyCheckRes = false)
        {
            NextFreeTimeOffset = -1;
            int start = pstart;
            int duration = pduration;
            planedDuration = duration;
            Dictionary<ScheduleActivity, bool> preActsUseRes = new Dictionary<ScheduleActivity, bool>();
            //计算前后使用相同资源的Activity，合并一起计算
            {
                ScheduleActivity preAct = activity.Previous;
                while (preAct != null)
                {
                    if (preAct.Resources.ContainsKey(Name) && preAct.Resources[Name] == count)
                    {
                        preActsUseRes[preAct] = true;
                        start = preAct.PlannedStart;
                        duration += preAct.PlannedDuration;
                    }
                    else
                    {
                        break;
                    }
                    preAct = preAct.Previous;
                }
                preAct = activity.Next;
                while (preAct != null)
                {
                    if (preAct.Resources.ContainsKey(Name) && preAct.Resources[Name] == count)
                    {
                        duration += preAct.Duration;
                    }
                    else
                    {
                        break;
                    }
                    preAct = preAct.Next;
                }
            }
            int end = start + duration;
            int[] units = new int[count];
            for (int i = 0; i < units.Length; i++)
            {
                units[i] = -1;
            }
            int current = 0;
            //计算资源使用情况
            Dictionary<int, int> intersectDistWithUnit = new Dictionary<int, int>();
            foreach (ResourceReservation u in Reservations)
            {
                //排除前边连续使用相同资源的Activity
                if (!preActsUseRes.ContainsKey(u.Activity))
                {
                    //if ((start >= u.PlannedStart && start <= (u.PlannedStart + u.PlannedDuration - 1)) ||
                    //    (end >= u.PlannedStart && end <= (u.PlannedStart + u.PlannedDuration - 1)) ||
                    //    (u.PlannedStart >= start && u.PlannedStart <= end) || (u.PlannedStart + u.PlannedDuration - 1 >= start && u.PlannedStart + u.PlannedDuration - 1 <= end))
                    int intersecteDistance = 0;
                    if ((intersecteDistance=GetIntersetLength(start, end, u.PlannedStart, u.PlannedDuration + u.PlannedStart))>0)
                    {
                        
                        for (int i = 0; i < u.Uint.Length; i++)
                        {
                            if (intersectDistWithUnit.ContainsKey(u.Uint[i]))
                                intersectDistWithUnit[u.Uint[i]] += intersecteDistance;
                            else
                                intersectDistWithUnit[u.Uint[i]] = intersecteDistance;
                        }
                    }
                }
            }
            //计算前一个Activity资源，如果可能重复使用
            bool canUsePreActRes = false;
            if (activity.Previous != null && activity.Previous.Reservations.ContainsKey(Name))
            {
                canUsePreActRes = true;
                int[] us = activity.Previous.Reservations[Name];
                int maxDecrease = 0;
                if (us != null &&  us.Length >= units.Length)
                {
                    for (int i = 0; i < us.Length; i++)
                    {
                        if (!this.IsUnitEnabled(us[i]))
                            canUsePreActRes = false;
                        if (intersectDistWithUnit.ContainsKey(us[i]))
                        {
                            int d = intersectDistWithUnit[us[i]];
                            if (intersectDistWithUnit[us[i]] <= pduration * SchedulerSetting.Instance.DefaultDecreaseRate)
                            {
                                if (maxDecrease < d)
                                    maxDecrease = d;
                            }
                            else
                                canUsePreActRes = false;
                        }
                    }
                    if (canUsePreActRes)
                    {
                        for (int i = 0; i < units.Length; i++)
                            units[i] = us[i];
                        planedDuration = pduration - maxDecrease;
                    }
                }
            }
            if (!canUsePreActRes)
            {
                int maxDecrease = 0;
                for (int i = 0; i < this.Count; i++)
                {
                    if ((!intersectDistWithUnit.ContainsKey(i) || intersectDistWithUnit[i] < pduration * SchedulerSetting.Instance.DefaultDecreaseRate) && this.IsUnitEnabled(i))
                    {
                        units[current] = i;
                        current++;
                        if (intersectDistWithUnit.ContainsKey(i))
                        {
                            int d = intersectDistWithUnit[i];
                            if (maxDecrease < d)
                                maxDecrease = d;
                        }
                        if (current == count)
                        {
                            planedDuration = pduration - maxDecrease;
                            break;
                        }
                    }
                }
                if (current == count)
                {
                    canUsePreActRes = true;
                }
            }
            if (canUsePreActRes)
                return units;
            if (onlyCheckRes)
                return null;
            //if (canUsePreActRes)
            //    return units;
            //如果以前计算的next free time可用，即返回
            if (activity.NextAvailableTime > activity.PlannedStart)
            {
                NextFreeTimeOffset = activity.NextAvailableTime - activity.PlannedStart;
                if (canUsePreActRes)
                {
                    return units;
                }
                else
                {
                    return null;
                }
            }
            {
                //计算最近资源空闲时间
                //如果资源ok，计算该activity下一次资源可用的offset
                int MaxUseTime = 0;
                foreach (ResourceReservation u in Reservations)
                {
                    //排除前边连续使用相同资源的Activity
                    int endTime = u.PlannedDuration + u.PlannedStart;
                    if (endTime > MaxUseTime)
                        MaxUseTime = endTime;
                }
                if (start > MaxUseTime)
                {
                    if (canUsePreActRes)
                    {
                        NextFreeTimeOffset =1;
                        return units;
                    }
                }
                int nstart = start;
                int nend = end;
                for (int s = nstart + 1; s <= MaxUseTime + 1; s++)
                {
                    start = s;
                    end = nend + s - nstart;
                    intersectDistWithUnit.Clear();
                    int intersectDistance = 0;
                    foreach (ResourceReservation u in Reservations)
                    {
                        //排除前边连续使用相同资源的Activity
                        if (!preActsUseRes.ContainsKey(u.Activity))
                        {
                            if ((intersectDistance=GetIntersetLength(start, end, u.PlannedStart, u.PlannedStart + u.PlannedDuration))>0)
                            {
                                for (int i = 0; i < u.Uint.Length; i++)
                                {
                                    if (intersectDistWithUnit.ContainsKey(u.Uint[i]))
                                        intersectDistWithUnit[u.Uint[i]] += intersectDistance;
                                    else
                                        intersectDistWithUnit[u.Uint[i]] = intersectDistance;
                                }
                            }
                        }
                    }
                    int usedCount = 0;
                    foreach(int v in intersectDistWithUnit.Values){
                        if (v > pduration * SchedulerSetting.Instance.DefaultDecreaseRate)
                            usedCount++;
                    }
                    NextFreeTimeOffset = s-nstart;
                    if (count <= (this.AvailableCount - usedCount))
                    {
                        if (canUsePreActRes)
                        {
                            return units;
                        }
                        if (!EnableActivityEnlarge)
                        {
                            return null;
                        }
                        //计算是否可以将前边的Activity最大延长距离
                        
                        int MaxOffset = 0;
                        ScheduleActivity preA = activity.Previous;
                        
                        while (preA != null)
                        {
                            MaxOffset += ((preA.MaxPlannedDuration > 0) ?
                                Math.Max(preA.MaxPlannedDuration - preA.PlannedDuration,0) :
                                Math.Max((int)(preA.Duration * SchedulerSetting.Instance.MaxPlantimeEnlarge - preA.PlannedDuration), 0));
                            preA = preA.Previous;
                        }
                        if (NextFreeTimeOffset >MaxOffset )
                        {
                            NextFreeTimeOffset = NextFreeTimeOffset - MaxOffset;
                            return null;
                        }
                        return null;
                    }
                }
            }
            return null;
        }
        public int[] Reserve(ScheduleActivity activity, int start, int duration, int count)
        {
            //int FreeTime = 0;
            //int planedDuration=0;
            int[] units = CheckReservation(activity, start, duration, count);
            if (units == null)
            {
                return null;
            }
            AddReservation(new ResourceReservation() { Activity = activity, Uint = units });
            return units;
        }
        void AddReservation(ResourceReservation reserv)
        {
            _reservation.Add(reserv);
            for (int i = 0; i < reserv.Uint.Length; i++)
            {
                UnitReservations ureservs = _unitReservations[reserv.Uint[i]];
                UnitReservation urv = new UnitReservation();
                urv.Resource = this;
                urv.Activity = reserv.Activity;
                urv.Uint = reserv.Uint[i];
                LinkedListNode<UnitReservation> node = ureservs.Last;
                LinkedListNode<UnitReservation> newNode = null;
                if (node == null)
                    newNode=ureservs.AddFirst(urv);
                else
                {
                    while (node != null)
                    {
                        if (urv.PlannedStart > node.Value.PlannedStart)
                        {
                            newNode = ureservs.AddAfter(node, urv);
                            break;
                        }
                        node = node.Previous;
                    }
                    if (node == null)
                        newNode = ureservs.AddFirst(urv);
                }
                reserv.Activity.UnitReservations.Add(newNode);
            }
        }
        public void RemoveReservationForActivity(ScheduleActivity a)
        {
            int index=-1;
            for(int i=0;i<_reservation.Count;i++)
            {
                ResourceReservation r = _reservation[i];
                if (r.Activity.Equals(a))
                {
                    index = i;
                    break;
                }
            }
            if(index>0)
                _reservation.RemoveAt(index);
            for (int i = 0; i < Count; i++)
            {
                UnitReservations urs = _unitReservations[i];
                LinkedListNode<UnitReservation> u = urs.First;
                while (u != null)
                {
                    LinkedListNode<UnitReservation> next = u.Next;
                    if (u.Value.Activity.Equals(a))
                        urs.Remove(u);
                    u = next;
                }
            }
        }
        Dictionary<int, ScheduleActivity> lockedBy = new Dictionary<int, ScheduleActivity>();
        //runtime
        public bool LockUnits(int[] units, ScheduleTask task, ScheduleActivity activity)
        {
            for (int i = 0; i < units.Length; i++)
            {
                if (_unitStatus[units[i]] != true)
                {
                    throw new Exception(string.Format("task {2} activity {3} resource {0} unit {1} is in using", Name, units[i], activity.Task.ID, activity.Name));
                }
                _unitStatus[units[i]] = false;
                lockedBy[units[i]] = activity;
            }
            if (ResourceLocked != null)
            {
                for (int i = 0; i < units.Length; i++)
                {
                    ResourceLocked(this, new ResourceEventArgs(Name, units[i], false, task, activity));
                }
            }
            return true;
        }
        public void FreeUnits(int[] units, ScheduleTask task, ScheduleActivity activity)
        {
            for (int i = 0; i < units.Length; i++)
            {
                if (_unitStatus[units[i]] != false)
                {                    
                    throw new Exception(string.Format("task {2} activity {3} resource {0} unit {1} is not in useing", Name, units[i], activity.Task.ID, activity.Name));
                }
                _unitStatus[units[i]] = true;
                lockedBy.Remove(units[i]);
            }
            if (ResourceFreed != null)
            {
                for (int i = 0; i < units.Length; i++)
                {
                    ResourceFreed(this, new ResourceEventArgs(Name, units[i],true,task, activity));
                }
            }
        }
        //event
        public SchedulerEventHandler<ResourceEventArgs> ResourceLocked;
        public SchedulerEventHandler<ResourceEventArgs> ResourceFreed;

    }
}
