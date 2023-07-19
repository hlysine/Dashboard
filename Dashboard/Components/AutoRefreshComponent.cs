using System;
using System.Timers;

namespace Dashboard.Components;

public abstract class AutoRefreshComponent : DashboardComponent
{
    public virtual TimeSpan ForegroundRefreshRate => TimeSpan.FromMinutes(2);
    public virtual TimeSpan BackgroundRefreshRate => TimeSpan.FromMinutes(30);

    private readonly Timer refreshTimer;

    protected AutoRefreshComponent()
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

    protected virtual TimeSpan GetRefreshRate()
    {
        return Foreground ? ForegroundRefreshRate : BackgroundRefreshRate;
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

    protected abstract void OnRefresh();
}
