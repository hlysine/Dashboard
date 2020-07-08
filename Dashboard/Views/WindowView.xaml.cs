using Dashboard.Components;
using Dashboard.Utilities;
using Dashboard.Views;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Dashboard.Utilities.Helper;

namespace Dashboard
{
    /// <summary>
    /// Interaction logic for WindowView.xaml
    /// </summary>
    public partial class WindowView : Window, IDashboardView<WindowContainer>
    {
        private DashboardManager manager = new DashboardManager();

        private Dictionary<DashboardComponent, UIElement> viewBindings = new Dictionary<DashboardComponent, UIElement>();

        public WindowContainer Component { get; private set; }

        public WindowView()
        {
            // TODO: check that Initialize() does return a WindowContainer
            WindowContainer window = (WindowContainer)manager.Initialize();
            Component = window;
            DataContext = window;

            InitializeComponent();

            Children_CollectionChanged(Component.Children, new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));


            Component.Children.CollectionChanged += Children_CollectionChanged;
        }

        private void Children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // TODO: complete other actions
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    e.NewItems.ForEach(x =>
                    {
                        var comp = (DashboardComponent)x;
                        UIElement elem = GetNewViewFor(comp);
                        viewBindings.Add(comp, elem);
                        root.Children.Add(elem);
                    });
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    e.OldItems.ForEach(x =>
                    {
                        var comp = (DashboardComponent)x;
                        root.Children.Remove(viewBindings[comp]);
                        viewBindings.Remove(comp);
                    });
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    e.OldItems.ForEach(x =>
                    {
                        var comp = (DashboardComponent)x;
                        root.Children.Remove(viewBindings[comp]);
                        viewBindings.Remove(comp);
                    });
                    e.NewItems.ForEach(x =>
                    {
                        var comp = (DashboardComponent)x;
                        UIElement elem = GetNewViewFor(comp);
                        viewBindings.Add(comp, elem);
                        root.Children.Add(elem);
                    });
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    viewBindings.Clear();
                    root.Children.Clear();
                    Component.Children.ForEach(x =>
                    {
                        UIElement elem = GetNewViewFor(x);
                        viewBindings.Add(x, elem);
                        root.Children.Add(elem);
                    });
                    break;
            }
        }

        private UIElement GetNewViewFor(DashboardComponent component)
        {
            // TODO: remove BaseType? chain
            var classList = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                             from assemblyType in domainAssembly.GetTypes()
                             where assemblyType.IsSubclassOf(typeof(DashboardViewBase))
                                && (assemblyType.BaseType?.BaseType?.GenericTypeArguments.Contains(component.GetType())).GetValueOrDefault()
                                && !assemblyType.IsAbstract
                             select assemblyType).ToArray();
            return (UIElement)Activator.CreateInstance(classList.First(), component);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ToolWindowHelper.SetToolWindow(this);
        }

        /// <summary>Brings main window to foreground.</summary>
        public void BringToForeground()
        {
            if (WindowState == WindowState.Minimized || Visibility == Visibility.Hidden)
            {
                Show();
                WindowState = WindowState.Normal;
            }

            // According to some sources these steps gurantee that an app will be brought to foreground.
            Activate();
            Topmost = true;
            Topmost = false;
            Focus();
        }
    }
}
