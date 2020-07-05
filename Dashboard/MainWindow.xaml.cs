using Dashboard.Components;
using Dashboard.Utilities;
using System;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ComponentManager manager = new ComponentManager();

        public MainWindow()
        {
            DataContext = manager;
            manager.Components.CollectionChanged += Components_CollectionChanged;
            InitializeComponent();
        }

        private void Components_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                e.NewItems.ForEach(x => root.Children.Add((UIElement)x));
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ToolWindowHelper.SetToolWindow(this);
        }
    }
}
