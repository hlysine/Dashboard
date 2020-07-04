using Dashboard.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Controls;

namespace Dashboard.Components
{
    public abstract class DashboardComponent<TController> : UserControl where TController : DashboardController, new()
    {
        public TController Controller { get; private set; }


        public DashboardComponent(ComponentManager manager) : base()
        {
            if (manager != null)
            {
                Controller = manager.GetController<TController>();
                DataContext = Controller;
            }
        }
    }
}
