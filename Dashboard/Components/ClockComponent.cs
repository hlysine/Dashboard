using System;

namespace Dashboard.Components;

public class ClockComponent : AutoRefreshComponent
{
    public override string DefaultName => "Clock";

    public DateTime Time
    {
        get { return DateTime.Now; }
    }

    public override TimeSpan ForegroundRefreshRate => TimeSpan.FromMilliseconds(100);

    public ClockComponent()
    {
        Loaded = true;
    }

    protected override void OnInitializationComplete()
    {
        StartAutoRefresh();
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