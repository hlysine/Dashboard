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
    /// Interaction logic for GoogleCalendarComponent.xaml
    /// </summary>
    public partial class GoogleCalendarComponent : GoogleCalendarComponentBase
    {
        public GoogleCalendarComponent(ComponentManager manager = null) : base(manager)
        {
            InitializeComponent();
            Load();
        }
    }

    public class GoogleCalendarComponentBase : DashboardComponent<GoogleCalendarController>
    {
        protected GoogleCalendarComponentBase(ComponentManager manager) : base(manager)
        {

        }

        protected GoogleCalendarComponentBase() : this(null)
        {

        }
    }
}
