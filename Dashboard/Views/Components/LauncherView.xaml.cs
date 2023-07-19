using System.Windows.Input;
using Dashboard.Components;

namespace Dashboard.Views.Components;

/// <summary>
/// Interaction logic for LauncherView.xaml
/// </summary>
public partial class LauncherView : LauncherViewBase
{
    public LauncherView(LauncherComponent component = null) : base(component)
    {
        InitializeComponent();
        Load();
    }

    private void TextBox_KeyUp(object sender, KeyEventArgs e)
    {
        // No command binding for pressing the enter key
        // Do it from code behind
        if (e.Key == Key.Enter)
        {
            ICommand cmd = ((LauncherComponent)DataContext).LaunchCommand;
            if (cmd.CanExecute(null))
            {
                cmd.Execute(null);
            }
        }
    }
}

public abstract class LauncherViewBase : DashboardView<LauncherComponent>
{
    protected LauncherViewBase(LauncherComponent component) : base(component)
    {

    }

    protected LauncherViewBase() : this(null)
    {

    }
}