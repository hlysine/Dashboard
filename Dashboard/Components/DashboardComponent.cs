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

        private object loadedContent;

        public DashboardComponent(ComponentManager manager)
        {
            if (manager != null)
            {
                Controller = manager.GetController<TController>();
                DataContext = Controller;
            }
        }

        protected void Load()
        {
            if (!Controller.Loaded)
            {
                Controller.FinishedLoading += Controller_FinishedLoading;
                loadedContent = Content;

                ProgressBar loadingBar = new ProgressBar();
                loadingBar.Style = (System.Windows.Style)FindResource("MaterialDesignCircularProgressBar");
                loadingBar.Value = 0;
                loadingBar.IsIndeterminate = true;

                Content = loadingBar;
            }
        }

        private void Controller_FinishedLoading(object sender, EventArgs e)
        {
            Content = loadedContent;
        }
    }
}
