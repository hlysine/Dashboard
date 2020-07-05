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
    /// Interaction logic for GoogleGmailComponent.xaml
    /// </summary>
    public partial class GoogleGmailComponent : GoogleGmailComponentBase
    {
        public GoogleGmailComponent(ComponentManager manager = null) : base(manager)
        {
            InitializeComponent();
            Load();
        }
    }

    public class GoogleGmailComponentBase : DashboardComponent<GoogleGmailController>
    {
        protected GoogleGmailComponentBase(ComponentManager manager) : base(manager)
        {

        }

        protected GoogleGmailComponentBase() : this(null)
        {

        }
    }
}
