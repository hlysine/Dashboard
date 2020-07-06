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
    /// Interaction logic for OsuComponent.xaml
    /// </summary>
    public partial class OsuComponent : OsuComponentBase
    {
        public OsuComponent(ComponentManager manager = null) : base(manager)
        {
            InitializeComponent();
            Load();
        }
    }

    public class OsuComponentBase : DashboardComponent<OsuController>
    {
        protected OsuComponentBase(ComponentManager manager) : base(manager)
        {

        }

        protected OsuComponentBase() : this(null)
        {

        }
    }
}
