using System;
using System.Timers;

namespace Dashboard.Components
{
    public abstract class AutoRefreshComponent : DashboardComponent
    {
        public virtual TimeSpan ForegroundRefreshRate => TimeSpan.FromMinutes(2);
        public virtual TimeSpan BackgroundRefreshRate => TimeSpan.FromMinutes(30);

        private Timer refreshTimer;

        public AutoRefreshComponent()
        {
            refreshTimer = new Timer();
            refreshTimer.Interval = GetRefreshRate().TotalMilliseconds;
            refreshTimer.AutoReset = true;
            refreshTimer.Elapsed += RefreshTimer_Elapsed;
        }

        private void RefreshTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Loaded)
                OnRefresh();
        }

        //public override void Initialize()
        //{
        //    base.Initialize();
        //}

        protected virtual TimeSpan GetRefreshRate()
        {
            if (Foreground)
                return ForegroundRefreshRate;
            else
                return BackgroundRefreshRate;
        }

        protected override void OnForegroundChanged()
        {
            refreshTimer.Interval = GetRefreshRate().TotalMilliseconds;
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
