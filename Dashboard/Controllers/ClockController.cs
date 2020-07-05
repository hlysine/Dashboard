using Dashboard.Config;
using Dashboard.ServiceProviders;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Dashboard.Controllers
{
    public class ClockController : DashboardController
    {
        public DateTime Time
        {
            get { return DateTime.Now; }
        }

        private Timer timer;

        public ClockController()
        {
            timer = new Timer(_ => NotifyChanged(nameof(Time)), null, 0, 100);
            Loaded = true;
        }
    }
}
