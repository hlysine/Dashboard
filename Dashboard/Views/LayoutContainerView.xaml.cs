using Dashboard.Components;
using System.Windows;

namespace Dashboard.Views
{
    /// <summary>
    /// Interaction logic for LayoutContainerView.xaml
    /// </summary>
    public partial class LayoutContainerView : LayoutContainerViewBase
    {
        public LayoutContainerView(LayoutContainer component = null) : base(component)
        {
            InitializeComponent();
            Load();
        }

        protected override void AddView(DashboardViewBase element)
        {
            root.Children.Add(element);
        }

        protected override void ClearView()
        {
            root.Children.Clear();
        }

        protected override void RemoveView(DashboardViewBase element)
        {
            root.Children.Remove(element);
        }
    }

    public abstract class LayoutContainerViewBase : DashboardContainerView<LayoutContainer>
    {
        protected LayoutContainerViewBase(LayoutContainer component) : base(component)
        {

        }

        protected LayoutContainerViewBase() : this(null)
        {

        }
    }
}
