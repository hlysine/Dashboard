using Dashboard.Components;

namespace Dashboard.Views.Components;

/// <summary>
/// Interaction logic for ClockView.xaml
/// </summary>
public partial class ClockView : ClockViewBase
{
    public ClockView(ClockComponent component = null) : base(component)
    {
        InitializeComponent();
        Load();
    }
}

public abstract class ClockViewBase : DashboardView<ClockComponent>
{
    protected ClockViewBase(ClockComponent component) : base(component)
    {

    }

    protected ClockViewBase() : this(null)
    {

    }
}