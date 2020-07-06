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

        public override TimeSpan ForegroundRefreshRate => TimeSpan.FromMilliseconds(100);

        public ClockController()
        {
            Loaded = true;
        }

        public override void OnRefresh()
        {
            NotifyChanged(nameof(Time));
        }
    }
}
