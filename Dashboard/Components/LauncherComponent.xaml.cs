using Dashboard.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Dashboard.Components
{
    /// <summary>
    /// Interaction logic for ClockComponent.xaml
    /// </summary>
    public partial class LauncherComponent : LauncherComponentBase
    {
        public LauncherComponent(ComponentManager manager = null) : base(manager)
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
                var cmd = ((LauncherController)DataContext).LaunchCommand;
                if (cmd.CanExecute(null))
                {
                    cmd.Execute(null);
                }
            }
        }
    }

    public class LauncherComponentBase : DashboardComponent<LauncherController>
    {
        protected LauncherComponentBase(ComponentManager manager) : base(manager)
        {

        }

        protected LauncherComponentBase() : this(null)
        {

        }
    }
}
