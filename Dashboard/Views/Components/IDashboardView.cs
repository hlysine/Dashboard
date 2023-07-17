using Dashboard.Components;

namespace Dashboard.Views.Components
{
    public interface IDashboardView<TComponent> where TComponent : DashboardComponent, new()
    {
        public TComponent Component { get; }
    }
}
