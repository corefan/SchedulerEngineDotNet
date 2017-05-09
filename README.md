# SchedulerEngineDotNet
This is a simple scheduler engine for robot control in C#. With it you can easily enable your software processing multiple task parallel on your instrument.

How to use it,

1. Create the scheduler engine
```csharp
SchedulerEngineEx scheduler = new SchedulerEngineEx();
```
2. Define the resource
```csharp
            scheduler.Register("STAR", 1, Color.Blue.ToArgb());
            scheduler.Register("iSWAP", 1, Color.Red.ToArgb());
            scheduler.Register("Incubator", 10, Color.Green.ToArgb());
            scheduler.Register("Reader", 1, Color.Yellow.ToArgb());
```
3. Define the assay
```csharp
    class Task1 : TaskEx
    {
        public override void Run()
        {
            StartTask("Assay1");
            if (Activity("pipette", 60, new string[] { "STAR" }, new int[] { 1 }, Color.Blue))
            {
                //your code here to control the instrument
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
```    
4. activate the assay and schedule the workflow
```csharp
 scheduler.Activate(new Task1());
 ```
 5. start running
 ```csharp
 scheduler.Run();
 ```
 we also write a simple GUI to display the task and resource, which can easily shown in the WinForm
 ```csharp
 Scheduler.SchedulerView view = new Scheduler.SchedulerView();
 view.Init(scheduler);
 ```
 ScreenShot
 <img src="Screen1.PNG"/>
