using Dashboard.Components;

namespace Dashboard.Views.Components
{
    public interface IDashboardView<out TComponent> where TComponent : DashboardComponent, new()
    {
        public TComponent Component { get; }
    }
}
