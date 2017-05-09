using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Scheduler;

namespace TestSchedulerEx
{
    public partial class Form1 : Form
    {
        Scheduler.SchedulerView view = new Scheduler.SchedulerView();
        SchedulerEngineEx scheduler = new SchedulerEngineEx();
        public Form1()
        {
            InitializeComponent();
            panel1.Controls.Add(view);
            view.ScaleFactor = 1;
            view.Dock = DockStyle.Fill;

            scheduler.Register("STAR", 1, Color.Blue.ToArgb());
            scheduler.Register("iSWAP", 1, Color.Red.ToArgb());
            scheduler.Register("Incubator", 10, Color.Green.ToArgb());
            scheduler.Register("Reader", 1, Color.Yellow.ToArgb());

            view.Init(scheduler);
        }

        Random r = new Random();
        private void ButtonNew_Click(object sender, EventArgs e)
        {
            int total = r.Next(8);
            for (int i = 0; i < total; i++)
            {
                if (r.Next(10) < 5)
                    scheduler.Activate(new Task1());
                else
                    scheduler.Activate(new Task2());
            }
            scheduler.Schedule();
        }

        private void ButtonSchedule_Click(object sender, EventArgs e)
        {
        }

        private void ButtonRun_Click(object sender, EventArgs e)
        {
            scheduler.Run();
        }
    }
    class Task1 : TaskEx
    {
        public override void Run()
        {
            StartTask("Assay1");
            if (Activity("pipette", 60, new string[] { "STAR" }, new int[] { 1 }, Color.Blue))
            {
            }
            if (Activity("transport", 20, new string[] { "iSWAP", "Incubator" }, new int[] { 1, 1 }, Color.Red))
            {
            }
            if (Activity("incubation", 300, new string[] { "Incubator" }, new int[] { 1 }, Color.Green))
            {
            }
            if (Activity("transport", 20, new string[] { "iSWAP", "Incubator", "Reader" }, new int[] { 1, 1, 1 }, Color.Red))
            {
            }
            if (Activity("reader", 60, new string[] { "Reader" }, new int[] { 1 }, Color.Yellow))
            {
            }
            EndTask();
        }
    }
    class Task2 : TaskEx
    {
        public override void Run()
        {
            StartTask("Assay2");
            if (Activity("pipette", 60, new string[] { "STAR" }, new int[] { 1 }, Color.Blue))
            {
            }
            if (Activity("transport", 20, new string[] { "iSWAP", "Incubator" }, new int[] { 1, 1 }, Color.Red))
            {
            }
            if (Activity("incubation", 420, new string[] { "Incubator" }, new int[] { 1 }, Color.Green))
            {
            }
            if (Activity("transport", 20, new string[] { "iSWAP", "Incubator" }, new int[] { 1, 1 }, Color.Red))
            {
            }
            if (Activity("pipette", 60, new string[] { "STAR" }, new int[] { 1 }, Color.Blue))
            {
            }
            if (Activity("transport", 20, new string[] { "iSWAP", "Incubator", "Reader" }, new int[] { 1, 1, 1 }, Color.Red))
            {
            }
            if (Activity("reader", 60, new string[] { "Reader" }, new int[] { 1 }, Color.Yellow))
            {
            }
            EndTask();
        }
    }
}
