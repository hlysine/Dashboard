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
    public class TabbedContainer : DashboardContainer
    {
        public override string DefaultName => "Tabbed Container";

        public override string Name => CustomName.IsNullOrEmpty() ? Children[ActiveTabIndex].Name : CustomName;

        private int activeTabIndex;

        public int ActiveTabIndex
        {
            get => activeTabIndex;
            set
            {
                SetAndNotify(ref activeTabIndex, value, new[] { nameof(Name) });
                activeTabChanged();
            }
        }

        [PersistentConfig]
        public int DefaultTabIndex { get; set; }

        public TabbedContainer()
        {
        }

        private void activeTabChanged()
        {
            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].ThisForeground = i == ActiveTabIndex;
            }
        }

        protected override void OnInitializationComplete()
        {
            ActiveTabIndex = DefaultTabIndex;
            Loaded = true;
        }
    }
}
