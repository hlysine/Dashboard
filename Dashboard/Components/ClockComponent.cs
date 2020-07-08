using Dashboard.Config;
using Dashboard.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Dashboard.Components
{
    public class ClockComponent : AutoRefreshComponent
    {
        public DateTime Time
        {
            get { return DateTime.Now; }
        }

        public override TimeSpan ForegroundRefreshRate => TimeSpan.FromMilliseconds(100);

        public ClockComponent()
        {
            Loaded = true;
        }

        protected override void OnForegroundChanged()
        {
            base.OnForegroundChanged();
            OnRefresh();
        }

        protected override void OnRefresh()
        {
            NotifyChanged(nameof(Time));
        }
    }
}
