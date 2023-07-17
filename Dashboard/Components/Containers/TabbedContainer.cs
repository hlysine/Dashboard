using Dashboard.Config;
using Dashboard.Utilities;

namespace Dashboard.Components.Containers
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
