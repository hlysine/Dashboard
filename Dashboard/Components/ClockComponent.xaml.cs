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
    public partial class ClockComponent : ClockComponentBase
    {
        public ClockComponent(ComponentManager manager = null) : base(manager)
        {
            InitializeComponent();
        }
    }

    public class ClockComponentBase : DashboardComponent<ClockController>
    {
        protected ClockComponentBase(ComponentManager manager) : base(manager)
        {

        }

        protected ClockComponentBase() : this(null)
        {

        }
    }
}
