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
    /// Interaction logic for SpotifyComponent.xaml
    /// </summary>
    public partial class SpotifyComponent : SpotifyComponentBase
    {
        public SpotifyComponent(ComponentManager manager = null) : base(manager)
        {
            InitializeComponent();
        }
    }

    public class SpotifyComponentBase : DashboardComponent<SpotifyController>
    {
        protected SpotifyComponentBase(ComponentManager manager) : base(manager)
        {

        }

        protected SpotifyComponentBase() : this(null)
        {

        }
    }
}
