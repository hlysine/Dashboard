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

    public override void InitializeDependencies()
    {
        Children.ForEach(x => x.InitializeDependencies());
        OnInitializeDependencies();
    }

    public override void InitializeSelf()
    {
        Children.ForEach(x => x.InitializeSelf());
        OnInitializeSelf();
    }

    public override void ForegroundChanged()
    {
        Children.ForEach(x => x.ForegroundChanged());
        OnForegroundChanged();
    }
}
