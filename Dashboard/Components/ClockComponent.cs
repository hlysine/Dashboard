using System;

namespace Dashboard.Components;

public class ClockComponent : AutoRefreshComponent
{
    protected override string DefaultName => "Clock";

    public DateTime Time => DateTime.Now;

    public override TimeSpan ForegroundRefreshRate => TimeSpan.FromMilliseconds(100);

    public ClockComponent()
    {
        Loaded = true;
    }

    protected override void OnInitializeSelf()
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
