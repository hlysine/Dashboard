using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using Dashboard.Components;

namespace Dashboard.Views
{
    public interface IDashboardView<TComponent> where TComponent : DashboardComponent, new()
    {
        public TComponent Component { get; }
    }
}
