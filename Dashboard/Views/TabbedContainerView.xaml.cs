using Dashboard.Components;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Dashboard.Views
{
    /// <summary>
    /// Interaction logic for TabbedContainerView.xaml
    /// </summary>
    public partial class TabbedContainerView : TabbedContainerViewBase
    {
        public TabbedContainerView(TabbedContainer component = null) : base(component)
        {
            InitializeComponent();
            Load();
        }

        protected override void AddView(DashboardViewBase element)
        {
            var tab = new TabItem();
            tab.Tag = element;
            tab.Content = element;
            Binding binding = new Binding("Name");
            binding.Source = element.DataContext;
            tab.SetBinding(TabItem.HeaderProperty, binding);
            root.Items.Add(tab);
        }

        protected override void ClearView()
        {
            root.Items.Clear();
        }

        protected override void RemoveView(DashboardViewBase element)
        {
            for (int i = root.Items.Count - 1; i >= 0; i--)
            {
                if (root.Items[i] is TabItem item)
                {
                    if (item.Tag == element) root.Items.RemoveAt(i);
                }
            }
        }
    }

    public abstract class TabbedContainerViewBase : DashboardContainerView<TabbedContainer>
    {
        protected TabbedContainerViewBase(TabbedContainer component) : base(component)
        {

        }

        protected TabbedContainerViewBase() : this(null)
        {

        }
    }
}
