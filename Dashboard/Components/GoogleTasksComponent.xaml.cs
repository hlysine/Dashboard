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
    /// Interaction logic for GoogleTasksComponent.xaml
    /// </summary>
    public partial class GoogleTasksComponent : GoogleTasksComponentBase
    {
        public GoogleTasksComponent(ComponentManager manager = null) : base(manager)
        {
            InitializeComponent();
        }
    }

    public class GoogleTasksComponentBase : DashboardComponent<GoogleTasksController>
    {
        protected GoogleTasksComponentBase(ComponentManager manager) : base(manager)
        {

        }

        protected GoogleTasksComponentBase() : this(null)
        {

        }
    }
}
