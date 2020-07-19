using Dashboard.Components.Containers;
using Dashboard.Config;
using Dashboard.Services;
using Dashboard.Utilities;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Dashboard.Components
{
    public class LayoutContainer : DashboardContainer
    {
        public override string DefaultName => "Layout Container";

        public LayoutContainer()
        {
            Loaded = true;
        }
    }
}
