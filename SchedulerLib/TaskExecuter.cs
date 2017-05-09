using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Scheduler
{
    public class TaskExecuter
    {
        ScheduleTask _task;
        Scheduler _scheduler;
        ManualResetEvent evnt = new ManualResetEvent(false);
        public TaskExecuter(Scheduler scheduler, ScheduleTask task)
        {
            _task = task;
            _scheduler = scheduler;
        }
        public void Execute()
        {
            new Thread(new ThreadStart(Run)).Start();
        }
        ScheduleActivity currentActivity = null;
        void Run()
        {
            for (int i = 0; i < _task.Activities.Length; i++)
            {
                ScheduleActivity ac = _task.Activities[i];
                currentActivity = ac;
                while (true)
                {
                    bool canRun = true;
                    Dictionary<LinkedListNode<UnitReservation>, bool> currentUnit = new Dictionary<LinkedListNode<UnitReservation>, bool>();
                    for (int j = 0; j < ac.UnitReservations.Count; j++)
                    {
                        LinkedListNode<UnitReservation> pre = ac.UnitReservations[j].Previous;
                        currentUnit[ac.UnitReservations[j]] = true;
                        if (pre != null && (pre.Value.Activity.Status != Status.Processed || _scheduler.Current<ac.PlannedStart))
                            canRun = false;
                    }
                    if (canRun)
                    {
                        ac.ActualStart = _scheduler.Current;
                        ac.Status = Status.Running;
                        Console.WriteLine("task {0} activity {1} is running", _task.ID, ac.Name);
                        ac.Execute();
                        while (true)
                        {
                            //检查下一个Activity是否可以开始,计算后便所有Activity的开始和结束时间
                            ScheduleActivity next = ac.Next;
                            bool canRunNext = true;
                            if (next != null && next.UnitReservations.Count > 0)
                            {
                                for (int j = 0; j < next.UnitReservations.Count; j++)
                                {
                                    LinkedListNode<UnitReservation> nur = next.UnitReservations[j].Previous;
                                    if (nur!=null && !currentUnit.ContainsKey(nur))
                                    {
                                        if (nur.Value.Activity.Status != Status.Processed)
                                            canRunNext = false;
                                    }
                                }
                            }
                            if (canRunNext)
                            {
                                //是否提前结束
                                ac.ActualDuration = _scheduler.Current-ac.ActualStart-1;
                                int now=_scheduler.Current;
                                Console.WriteLine(now);
                                if (ac.PlannedStart + ac.PlannedDuration > now)
                                {
                                    //前移
                                    Console.WriteLine("<<<<<<<<<<<<<< " + (ac.PlannedStart + ac.PlannedDuration - now));
                                        int offset=ac.PlannedStart + ac.PlannedDuration - now;
                                        MoveActivityBackword(ac, offset);
                                }
                                else if (ac.ActualStart + ac.ActualDuration < now)
                                {
                                    //后移
                                    Console.WriteLine(">>>>>>>>>>>>>>> " + (ac.PlannedStart + ac.PlannedDuration - now));
                                    
                                        int offset=now - ac.PlannedStart - ac.PlannedDuration;
                                        MoveActibityForward(ac,offset);
                                }
                                break;
                            }
                            Thread.Sleep(300);
                        }
                        ac.Status = Status.Processed;
                        break;
                    }
                    else
                    {
                        Thread.Sleep(300);
                    }
                }
            }
        }
        void MoveActivityBackword(ScheduleActivity act, int offset, int level=0)
        {
            int start = act.PlannedStart;
            int duration = act.PlannedDuration;
            level += 1;
            //处理后边的activity
            if (act.Next != null)
            {
                MoveActivityBackword(act.Next, offset, level);
            }
            if (act.UnitReservations != null)
            {
                for (int i = 0; i < act.UnitReservations.Count; i++)
                {
                    Dictionary<ScheduleActivity, bool> nextActs = new Dictionary<ScheduleActivity, bool>();
                    ScheduleActivity s = act.UnitReservations[i].Value.Activity;
                    if (nextActs.ContainsKey(s))
                    {
                        nextActs[s] = true;
                        int offset2 = s.PlannedStart - start - duration;
                        if (offset2 < offset)
                        {
                            MoveActivityBackword(s, offset - offset2, level);
                        }
                    }
                }
            }
            if (level != 1)
            {
                act.PlannedStart += offset;
            }
            else
                act.PlannedDuration += offset;
        }
        //>>>>>>>>>>>
        void MoveActibityForward(ScheduleActivity act, int offset, int level=0)
        {
            int start = act.PlannedStart;
            int duration = act.PlannedDuration;
            level += 1;
            //处理后边的activity
            if (act.Next != null)
            {
                MoveActibityForward(act.Next, offset, level);
            }
            if (act.UnitReservations != null)
            {
                for (int i = 0; i < act.UnitReservations.Count; i++)
                {
                    Dictionary<ScheduleActivity, bool> nextActs = new Dictionary<ScheduleActivity, bool>();
                    ScheduleActivity s = act.UnitReservations[i].Value.Activity;
                    if (nextActs.ContainsKey(s))
                    {
                        nextActs[s] = true;
                        int offset2 = s.PlannedStart - start - duration;
                        if (offset2<offset)
                        {
                            MoveActibityForward(s, offset-offset2, level);
                        }
                    }
                }
            }
            if (level != 1)
            {
                act.PlannedStart += offset;
            }
            else
                act.PlannedDuration += offset;
        }
    }
}
