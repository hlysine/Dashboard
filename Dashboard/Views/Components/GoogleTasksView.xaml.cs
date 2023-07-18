using Dashboard.Components;

namespace Dashboard.Views.Components;

/// <summary>
/// Interaction logic for GoogleTasksView.xaml
/// </summary>
public partial class GoogleTasksView : GoogleTasksViewBase
{
    public GoogleTasksView(GoogleTasksComponent component = null) : base(component)
    {
        InitializeComponent();
        Load();
    }
}

public abstract class GoogleTasksViewBase : DashboardView<GoogleTasksComponent>
{
    protected GoogleTasksViewBase(GoogleTasksComponent component) : base(component)
    {

    }

    protected GoogleTasksViewBase() : this(null)
    {

    }
}