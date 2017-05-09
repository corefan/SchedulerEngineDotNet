using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;

namespace Scheduler
{

    public class Task1 : Task
    {
        public Task1()
        {
            Activity t1 = new Activity() { Name = "Pipetting1", Color = Color.Blue, Duration = 10 };
            t1.Resources["STAR"] = 1;

            Activity t2 = new Activity() { Name = "Transport1", Color = Color.Red, Duration = 5 };
            t2.Resources["iSWAP"] = 1;
            t2.Resources["Incubator"] = 1;

            Activity t3 = new Activity() { Name = "Incubation", Color = Color.Green, Duration = 62 };
            t3.Resources["Incubator"] = 1;

            Activity t4 = new Activity() { Name = "Transport2", Color = Color.Red, Duration = 5 };
            t4.Resources["iSWAP"] = 1;
            t4.Resources["Incubator"] = 1;

            Activity t5 = new Activity() { Name = "Pipetting2", Color = Color.Blue, Duration = 10 };
            t5.Resources["STAR"] = 1;

            Activity t6 = new Activity() { Name = "Transport3", Color = Color.Red, Duration = 5 };
            t6.Resources["iSWAP"] = 1;
            t6.Resources["Reader"] = 1;

            Activity tr = new Activity() { Name = "Read", Color = Color.Orange, Duration = 10 };
            tr.Resources["Reader"] = 1;

            Activities = new Activity[] { t1, t2, t3, t4, t5, t6, tr };
            Name = "Assay1";
        }
    }

    public class Task3 : Task
    {
        public Task3()
        {
            Name = "Assay3";
            Activities = new Activity[] { 
                new Activity() { 
                    Name = "Pipetting", Color = Color.Blue, 
                    Duration = 6,
                    Resources={{"STAR",1}}
                },
                new Activity() { 
                    Name = "Transport1", Color = Color.Red, 
                    Duration = 3,
                    Resources={{"iSWAP",1},{"Shaker",1}}
                },
                new Activity() { 
                    Name = "Shaking", Color = Color.Yellow, 
                    Duration = 15,
                    Resources={{"Shaker",1}}
                },
                new Activity() { 
                    Name = "Transport2", Color = Color.Red, 
                    Duration = 3,
                    Resources={{"iSWAP",1},{"Shaker",1}}
                },
                new Activity() { 
                    Name = "Waiting", Color = Color.BlueViolet, 
                    Duration = 1,
                    MaxPlannedDuration=280
                },
                new Activity() { 
                    Name = "Transport3", Color = Color.Red, 
                    Duration = 3,
                    Resources={{"iSWAP",1},{"Reader",1}}
                },
                new Activity() { 
                    Name = "Reader", Color = Color.Orange, 
                    Duration = 13,
                    Resources={{"Reader",1}}
                }
            };
        }
    }
    public class Task2 : Task
    {
        public Task2()
        {
            Activity t1 = new Activity()
            {
                Name = "Pipetting1",
                Color = Color.Blue,
                Duration = 10,
                Resources = { { "STAR", 1 } }
            };

            Activity tw = new Activity() { Name = "Waiting", Color = Color.BlueViolet, Duration = 3 };

            Activity t2 = new Activity()
            {
                Name = "Transport1",
                Color = Color.Red,
                Duration = 5,
                Resources = { { "iSWAP", 1 }, { "Incubator", 1 } }
            };

            Activity t3 = new Activity()
            {
                Name = "Incubation",
                Color = Color.Green,
                Duration = 61,
                Resources = { { "Incubator", 1 } }
            };

            Activity t6 = new Activity()
            {
                Name = "Transport2",
                Color = Color.Red,
                Duration = 3,
                Resources = { { "iSWAP", 1 }, { "Incubator", 1 }  }
            };
            Activity t7 = new Activity()
            {
                Name = "Pipeeting2",
                Color = Color.Blue,
                Duration = 9,
                Resources = { { "STAR", 1 } }
            };

            Activity t4 = new Activity()
            {
                Name = "Transport3",
                Color = Color.Red,
                Duration = 5,
                Resources = { { "iSWAP", 1 }, { "Reader", 1 } }
            };

            Activity t5 = new Activity() { Name = "Read", Color = Color.Orange, Duration = 10 };
            t5.Resources["Reader"] = 1;

            Activities = new Activity[] { t1, tw, t2, t3,t6,t7, t4, t5 };
            Name = "Assay2";
        }
    }
    public class Task5 : Task
    {
        public Task5()
        {
            Name = "Assay5";
            Activity t1 = new Activity()
            {
                Name = "Pipetting1",
                Color = Color.Blue,
                Duration = 10,
                Resources = { { "STAR", 1 } }
            };

            Activity tw = new Activity() { Name = "Waiting", Color = Color.BlueViolet, Duration = 3 };

            Activity t2 = new Activity()
            {
                Name = "Transport1",
                Color = Color.Red,
                Duration = 5,
                Resources = { { "iSWAP", 1 }, { "Incubator", 1 } }
            };

            Activity t3 = new Activity()
            {
                Name = "Incubation",
                Color = Color.Green,
                Duration = 60,
                Resources = { { "Incubator",1  } }
            };

            Activity t4 = new Activity()
            {
                Name = "Transport2",
                Color = Color.Red,
                Duration = 5,
                Resources = { { "iSWAP", 1 }, { "Incubator", 1 }, { "Reader", 1 } }
            };

            Activity t5 = new Activity() { Name = "Read", Color = Color.Orange, Duration = 10 };
            t5.Resources["Reader"] = 1;

            Activities = new Activity[] { t1, tw, t2, t3, t4, t5 };
        }
    }
    public class Task4 : Task
    {
        public Task4()
        {
            Activity t1 = new Activity()
            {
                Name = "Pipetting1",
                Color = Color.Blue,
                Duration = 10,
                Resources = { { "STAR", 1 } }
            };

            Activity tw = new Activity() { Name = "Waiting", Color = Color.BlueViolet, Duration = 3 };

            Activity t2 = new Activity()
            {
                Name = "Transport1",
                Color = Color.Red,
                Duration = 5,
                Resources = { { "iSWAP", 1 }, { "Incubator", 1 } }
            };

            Activity t3 = new Activity()
            {
                Name = "Incubation",
                Color = Color.Green,
                Duration = 60,
                Resources = { { "Incubator", 1 } }
            };

            Activity t4 = new Activity()
            {
                Name = "Transport2",
                Color = Color.Red,
                Duration = 5,
                Resources = { { "iSWAP", 1 }, { "Incubator", 1 }, { "Reader", 1 } }
            };

            Activity t5 = new Activity() { Name = "Read", Color = Color.Orange, Duration = 10 };
            t5.Resources["Reader"] = 1;

            Activities = new Activity[] { t1, tw, t2, t3, t4, t5 };
            Name = "Assay2";
        }
    }
    public class TestSh
    {

        static void Main()
        {
            try
            {
                Test();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
        public static void Test()
        {
            EmbedScheduler scheduler = new EmbedScheduler();
            scheduler.Register(new Resource() { Name = "STAR", Count = 1, Color = Color.Blue });
            scheduler.Register(new Resource() { Name = "iSWAP", Count = 1, Color = Color.Red });
            scheduler.Register(new Resource() { Name = "Incubator", Count = 10, Color = Color.Green });
            scheduler.Register(new Resource() { Name = "Shaker", Count = 1, Color = Color.Yellow });
            scheduler.Register(new Resource() { Name = "Reader", Count = 1, Color = Color.Orange });
            IRuntimeResource reader = scheduler.GetResource("Reader");
            IRuntimeResource incubator = scheduler.GetResource("Incubator");
            //reader.DisableUnit(1);
            incubator.DisableUnit(3);

            scheduler.Activate(new Task1() { Name = "Assay1_1" });
            scheduler.Activate(new Task2() { Name = "Assay2_2" }, new Dictionary<int, TaskRelation>() { { 1, TaskRelation.AfterTaskEnd } });
            scheduler.Activate(new Task3() { Name = "Assay3_3" });
            scheduler.Activate(new Task2() { Name = "Assay2_4" }, new Dictionary<int, TaskRelation>() { { 3, TaskRelation.AfterTaskEnd } });
            scheduler.Activate(new Task4() { Name = "Assay4_5" }, new Dictionary<int, TaskRelation>() { { 1, TaskRelation.BeforeRunEnd } });
            scheduler.Activate(new Task1() { Name = "Assay1_6" });
            scheduler.Activate(new Task5() { Name = "Assay5_7" });
            scheduler.Activate(new Task1() { Name = "Assay1_8" }, new Dictionary<int, TaskRelation>() { { 1, TaskRelation.AfterTaskEnd } });
            scheduler.Activate(new Task3() { Name = "Assay3_9" });
            scheduler.Activate(new Task3() { Name = "Assay3_10" }, new Dictionary<int,TaskRelation>(){{9, TaskRelation.AfterTaskStart}});
            scheduler.Activate(new Task4() { Name = "Assay4_11" }, new Dictionary<int, TaskRelation>() { { 9, TaskRelation.AfterTaskStart } });
            scheduler.Activate(new Task1() { Name = "Assay1_12" });
            scheduler.Activate(new Task2() { Name = "Assay2_13" });
            scheduler.Activate(new Task3() { Name = "Assay3_14" });
            scheduler.Activate(new Task3() { Name = "Assay3_15" });
            scheduler.Activate(new Task4() { Name = "Assay4_16" });
            scheduler.Activate(new Task5() { Name = "Assay5_17" });
            scheduler.Activate(new Task2() { Name = "Assay2_18" });
            scheduler.Activate(new Task1() { Name = "Assay1_19" });
            scheduler.Activate(new Task2() { Name = "Assay2_20" });
            scheduler.Activate(new Task1() { Name = "Assay1_21" });
            scheduler.Activate(new Task5() { Name = "Assay5_22" });
            scheduler.Activate(new Task1() { Name = "Assay1_23" });
            scheduler.Activate(new Task2() { Name = "Assay2_24" });
            scheduler.Activate(new Task3() { Name = "Assay3_25" });
            scheduler.Activate(new Task1() { Name = "Assay1_26" });
            scheduler.Activate(new Task2() { Name = "Assay2_27" });
            scheduler.Activate(new Task1() { Name = "Assay1_28" });
            scheduler.Activate(new Task5() { Name = "Assay5_29" });
            scheduler.Activate(new Task3() { Name = "Assay3_30" }); ;
            scheduler.Activate(new Task4() { Name = "Assay4_31" });
            scheduler.Activate(new Task1() { Name = "Assay1_32" });
            scheduler.Activate(new Task3() { Name = "Assay3_33" });
            scheduler.Activate(new Task1() { Name = "Assay4_34" });
            scheduler.Activate(new Task2() { Name = "Assay2_35" });
            scheduler.Activate(new Task3() { Name = "Assay3_36" });
            scheduler.Activate(new Task5() { Name = "Assay5_37" });
            scheduler.Activate(new Task3() { Name = "Assay3_38" });
            scheduler.Activate(new Task1() { Name = "Assay1_39" });/*
            */
            DateTime d = DateTime.Now;
            scheduler.Schedule();
            Console.WriteLine("time used" + new TimeSpan(DateTime.Now.Ticks - d.Ticks).TotalMilliseconds);
            
            scheduler.Run();
            

            /*
            ScheduleTask t = scheduler.GetTask(3) as ScheduleTask;
            Console.WriteLine("detail for task" + t.ID);
            for (int i = 0; i < t.Activities.Length; i++)
            {
                ScheduleActivity ac = t.Activities[i];
                Console.WriteLine("activity {0} start={1} duration={2}", ac.Name, ac.PlannedStart, ac.PlannedDuration);
                for (int j = 0; j < ac.UnitReservations.Count; j++)
                {
                    UnitReservation ur = ac.UnitReservations[j].Value;
                    LinkedListNode<UnitReservation> node = ac.UnitReservations[j];
                    Console.WriteLine("    resource {0} unit {1} nextAct={4}", ur.Resource.Name, ur.Uint,
                        ur.PlannedStart, ur.PlannedDuration, node.Previous != null ? node.Previous.Value.Activity.Name + " of task " + node.Previous.Value.Activity.Task.ID : "non");
                }
            }
             */
            //scheduler.Run();
            /*
            ScheduleTask t = scheduler.GetTask(1) as ScheduleTask;
            t.Activities[0].Duration -= 10;
            DateTime d2 = DateTime.Now;
            scheduler.Schedule();
            Console.WriteLine("time used" + new TimeSpan(DateTime.Now.Ticks - d.Ticks).TotalMilliseconds);
            Bitmap map2 = scheduler.DrawGannt();
            //TestForm form2 = new TestForm();
            //form2.pictureBox1.Image = map2;
            //form2.Show();
             */
        }
    }
}
