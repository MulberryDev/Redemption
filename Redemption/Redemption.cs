using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Redemption
{
    public partial class Redemption : ServiceBase
    {
        ImageWatcher imageWatcher; 

        public Redemption()
        {
            InitializeComponent();
        }

        public void OnDebug()
        {
            OnStart(null);
            OnStop();
        }

        protected override void OnStart(string[] args)
        {
            imageWatcher = new ImageWatcher();
        }

        protected override void OnStop()
        {
            imageWatcher.Stop();
        }
    }
}
