using System.Windows.Controls;
using System.Windows.Data;
using Dashboard.Components.Containers;

namespace Dashboard.Views.Components;

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
        var tab = new TabItem
        {
            Tag = element,
            Content = element,
        };
        Binding binding = new("Name")
        {
            Source = element.DataContext,
        };
        tab.SetBinding(HeaderedContentControl.HeaderProperty, binding);
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
            if (root.Items[i] is not TabItem item)
                continue;

            if (Equals(item.Tag, element)) root.Items.RemoveAt(i);
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
