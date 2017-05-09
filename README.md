# SchedulerEngineDotNet
This is a simple scheduler engine for robot control in C#.

How to use it,

1. Create the scheduler engine<br/>
SchedulerEngineEx scheduler = new SchedulerEngineEx();<br/>

2. Define the resource<br/>
            scheduler.Register("STAR", 1, Color.Blue.ToArgb());<br/>
            scheduler.Register("iSWAP", 1, Color.Red.ToArgb());<br/>
            scheduler.Register("Incubator", 10, Color.Green.ToArgb());<br/>
            scheduler.Register("Reader", 1, Color.Yellow.ToArgb());<br/>

3. Define the assay<br/>
    class Task1 : TaskEx<br/>
    {<br/>
        public override void Run()<br/>
        {<br/>
            StartTask("Assay1");<br/>
            if (Activity("pipette", 60, new string[] { "STAR" }, new int[] { 1 }, Color.Blue))<br/>
            {<br/>
                //your code here to control the instrument<br/>
            }<br/>
            if (Activity("transport", 20, new string[] { "iSWAP", "Incubator" }, new int[] { 1, 1 }, Color.Red))<br/>
            {<br/>
            }<br/>
            if (Activity("incubation", 300, new string[] { "Incubator" }, new int[] { 1 }, Color.Green))<br/>
            {<br/>
            }<br/>
            if (Activity("transport", 20, new string[] { "iSWAP", "Incubator", "Reader" }, new int[] { 1, 1, 1 }, Color.Red))<br/>
            {<br/>
            }<br/>
            if (Activity("reader", 60, new string[] { "Reader" }, new int[] { 1 }, Color.Yellow))<br/>
            {<br/>
            }<br/>
            EndTask();<br/>
        }<br/>
    }<br/>
    
4. activate the assay and schedule the workflow<br/>
 scheduler.Activate(new Task1());<br/>
 
 5. start running<br/>
 scheduler.Run();<br/>
 
 we also write a simple GUI to display the task and resource, which can easily shown in the WinForm<br/>
 Scheduler.SchedulerView view = new Scheduler.SchedulerView();<br/>
 view.Init(scheduler);<br/>
