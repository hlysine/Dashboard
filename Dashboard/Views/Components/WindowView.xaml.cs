using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using Dashboard.Components;
using Dashboard.Components.Containers;
using Dashboard.Config;
using Dashboard.Utilities;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;

namespace Dashboard.Views.Components;

/// <summary>
/// Interaction logic for WindowView.xaml
/// </summary>
public partial class WindowView : Window, IDashboardView<WindowContainer>
{
    private readonly Dictionary<DashboardComponent, DashboardViewBase> viewBindings = new();

    public WindowContainer Component { get; private set; }

    public WindowView(WindowContainer component)
    {
        // Extract to DashboardWindowBase
        Component = component;
        DataContext = component;

        InitializeComponent();

        Children_CollectionChanged(Component.Children, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        changeColorScheme(Component.ColorScheme);


        Component.Children.CollectionChanged += Children_CollectionChanged;
        Component.PropertyChanged += Window_PropertyChanged;
    }

    private void Window_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        // Observe changes in ColorScheme since it can only be applied from code behind
        if (e.PropertyName == nameof(WindowContainer.ColorScheme))
        {
            changeColorScheme(Component.ColorScheme);
        }
    }

    [GeneratedRegex("(\\/MaterialDesignExtensions;component\\/Themes\\/MaterialDesign)((Light)|(Dark))Theme\\.")]
    private static partial Regex MaterialDesignThemeRegex();

    private static void changeColorScheme(ColorScheme scheme)
    {
        PaletteHelper paletteHelper = new();
        ITheme theme = paletteHelper.GetTheme();
        theme.SetBaseTheme(scheme.Theme == Config.Theme.Dark ? MaterialDesignThemes.Wpf.Theme.Dark : MaterialDesignThemes.Wpf.Theme.Light);
        IEnumerable<Swatch> swatches = new SwatchesProvider().Swatches.ToList();
        if (swatches.Where(x => x.ExemplarHue != null).Select(x => x.Name).Contains(scheme.PrimaryHue))
            theme.SetPrimaryColor(swatches.First(x => x.Name == scheme.PrimaryHue).ExemplarHue.Color);
        if (swatches.Where(x => x.AccentExemplarHue != null).Select(x => x.Name).Contains(scheme.AccentHue))
            theme.SetSecondaryColor(swatches.First(x => x.Name == scheme.AccentHue).AccentExemplarHue.Color);
        paletteHelper.SetTheme(theme);

        ResourceDictionary oldThemeResourceDictionary = Application.Current.Resources.MergedDictionaries
                                                                   .Where(resourceDictionary => resourceDictionary != null && resourceDictionary.Source != null)
                                                                   .SingleOrDefault(resourceDictionary => MaterialDesignThemeRegex().IsMatch(resourceDictionary.Source.OriginalString));

        var newThemeSource = $"pack://application:,,,/MaterialDesignExtensions;component/Themes/MaterialDesign{(scheme.Theme == Config.Theme.Dark ? "Dark" : "Light")}Theme.xaml";
        ResourceDictionary newThemeResourceDictionary = new() { Source = new Uri(newThemeSource) };

        Application.Current.Resources.MergedDictionaries.Remove(oldThemeResourceDictionary);
        Application.Current.Resources.MergedDictionaries.Add(newThemeResourceDictionary);
    }

    private void Children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                e.NewItems.ForEach(x =>
                {
                    var comp = (DashboardComponent)x;
                    DashboardViewBase elem = getNewViewFor(comp);
                    if (elem == null) return; //DEBUG
                    viewBindings.Add(comp, elem);
                    root.Children.Add(elem);
                });
                break;
            case NotifyCollectionChangedAction.Remove:
                e.OldItems.ForEach(x =>
                {
                    var comp = (DashboardComponent)x;
                    root.Children.Remove(viewBindings[comp]);
                    viewBindings.Remove(comp);
                });
                break;
            case NotifyCollectionChangedAction.Replace:
                e.OldItems.ForEach(x =>
                {
                    var comp = (DashboardComponent)x;
                    root.Children.Remove(viewBindings[comp]);
                    viewBindings.Remove(comp);
                });
                e.NewItems.ForEach(x =>
                {
                    var comp = (DashboardComponent)x;
                    DashboardViewBase elem = getNewViewFor(comp);
                    viewBindings.Add(comp, elem);
                    root.Children.Add(elem);
                });
                break;
            case NotifyCollectionChangedAction.Reset:
                viewBindings.Clear();
                root.Children.Clear();
                Component.Children.ForEach(x =>
                {
                    DashboardViewBase elem = getNewViewFor(x);
                    if (elem == null) return; //DEBUG
                    viewBindings.Add(x, elem);
                    root.Children.Add(elem);
                });
                break;
        }
    }

    // TODO: unify with DashboardViewBase
    private static DashboardViewBase getNewViewFor(DashboardComponent component)
    {
        // TODO: remove BaseType? chain
        Type[] classList = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
            from assemblyType in domainAssembly.GetTypes()
            where assemblyType.IsSubclassOf(typeof(DashboardViewBase))
                  && (assemblyType.BaseType?.BaseType?.GenericTypeArguments.Contains(component.GetType())).GetValueOrDefault()
                  && !assemblyType.IsAbstract
            select assemblyType).ToArray();
        Type target = classList.FirstOrDefault();
        if (target == null)
            return null;
        else
            return (DashboardViewBase)Activator.CreateInstance(target, component);
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

        // According to some sources these steps guarantee that an app will be brought to foreground.
        Activate();
        Topmost = true;
        Topmost = false;
        Focus();
    }
}
