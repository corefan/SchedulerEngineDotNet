using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scheduler
{
    public interface ITaskEx
    {
        void SetTaskControl(ITaskControl t);
        void Run();
    }
}
