using Dashboard.Config;
using Dashboard.Utilities;
using System.Collections.ObjectModel;

namespace Dashboard.Components.Containers;

public abstract class DashboardContainer : DashboardComponent
{
    public abstract override string DefaultName { get; }

    [PersistentConfig]
    public ObservableCollection<DashboardComponent> Children { get; set; } = new();

    protected DashboardContainer()
    {
        Children.CollectionChanged += Children_CollectionChanged;
    }

    private void Children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        e.OldItems?.ForEach(x =>
        {
            var comp = (DashboardComponent)x;
            if (comp.Parent == this) comp.Parent = null;
        });
        e.NewItems?.ForEach(x => ((DashboardComponent)x).Parent = this);
    }

    public override void GetServices(DashboardManager manager)
    {
        base.GetServices(manager);
        Children.ForEach(x => x.GetServices(manager));
    }

    public override void Initialize()
    {
        Children.ForEach(x => x.Initialize());
        OnInitialize();
    }

    public override void InitializationComplete()
    {
        Children.ForEach(x => x.InitializationComplete());
        OnInitializationComplete();
    }

    public override void ForegroundChanged()
    {
        Children.ForEach(x => x.ForegroundChanged());
        OnForegroundChanged();
    }
}
