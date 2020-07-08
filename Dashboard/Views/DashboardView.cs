using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using Dashboard.Components;

namespace Dashboard.Views
{
    public abstract class DashboardViewBase : UserControl
    {

    }
    public abstract class DashboardView<TComponent> : DashboardViewBase, IDashboardView<TComponent> where TComponent : DashboardComponent, new()
    {
        public TComponent Component { get; private set; }

        private object loadedContent;

        public DashboardView(TComponent component)
        {
            Component = component;
            DataContext = Component;
        }

        protected void Load()
        {
            if (!Component.Loaded)
            {
                Component.FinishedLoading += Component_FinishedLoading;
                loadedContent = Content;

                ProgressBar loadingBar = new ProgressBar();
                loadingBar.Style = (System.Windows.Style)FindResource("MaterialDesignCircularProgressBar");
                loadingBar.Value = 0;
                loadingBar.IsIndeterminate = true;

                Content = loadingBar;
            }
        }

        private void Component_FinishedLoading(object sender, EventArgs e)
        {
            Content = loadedContent;
        }
    }
}
