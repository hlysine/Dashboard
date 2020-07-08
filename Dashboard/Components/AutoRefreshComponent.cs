using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Dashboard.Components
{
    public abstract class AutoRefreshComponent : DashboardComponent
    {
        public virtual TimeSpan ForegroundRefreshRate => TimeSpan.FromMinutes(2);
        public virtual TimeSpan BackgroundRefreshRate => TimeSpan.FromMinutes(30);

        private DispatcherTimer refreshTimer;

        public AutoRefreshComponent()
        {
            refreshTimer = new DispatcherTimer();
            refreshTimer.Interval = GetRefreshRate();
            refreshTimer.Tick += RefreshTimer_Tick;
            refreshTimer.Stop();
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            OnRefresh();
        }

        protected virtual TimeSpan GetRefreshRate()
        {
            if (Foreground)
                return ForegroundRefreshRate;
            else
                return BackgroundRefreshRate;
        }

        protected override void OnForegroundChanged()
        {
            refreshTimer.Interval = GetRefreshRate();
        }

        protected void StartAutoRefresh()
        {
            refreshTimer.Start();
        }

        protected void StopAutoRefresh()
        {
            refreshTimer.Stop();
        }

        protected virtual void OnRefresh()
        {

        }
    }
}
