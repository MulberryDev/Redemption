using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Redemption
{
    static class Program
    {
        static void Main()
        {
            #if DEBUG
                Redemption redemption = new Redemption();
                redemption.OnDebug();
                System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
            #else
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] 
                { 
                    new Redemption() 
                };
                ServiceBase.Run(ServicesToRun);
            #endif
        }
    }
}
