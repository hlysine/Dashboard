using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Dashboard.Components;
using Dashboard.Utilities.Converters;

namespace Dashboard.Views.Components;

public abstract class DashboardViewBase : UserControl
{
}
public abstract class DashboardView<TComponent> : DashboardViewBase, IDashboardView<TComponent> where TComponent : DashboardComponent, new()
{
    public TComponent Component { get; private set; }

    private object loadedContent;

    protected DashboardView(TComponent component)
    {
        Component = component;
        DataContext = Component;
    }

    protected void Load()
    {
        loadedContent = Content;
        if (!Component.Loaded)
        {
            Component.FinishedLoading += Component_FinishedLoading;

            ProgressBar loadingBar = new()
            {
                Style = (Style)FindResource("MaterialDesignCircularProgressBar"),
                Value = 0,
                IsIndeterminate = true,
            };

            Content = wrapWithTitle(loadingBar);
        }
        else
        {
            Content = null;
            Content = wrapWithTitle((UIElement)loadedContent);
        }
    }

    private void Component_FinishedLoading(object sender, EventArgs e)
    {
        Content = wrapWithTitle((UIElement)loadedContent);
    }

    private UIElement wrapWithTitle(UIElement content)
    {
        Grid grid = new();
        grid.RowDefinitions.Add(new RowDefinition
            { Height = new GridLength(0, GridUnitType.Auto) });
        grid.RowDefinitions.Add(new RowDefinition
            { Height = new GridLength(0, GridUnitType.Auto) });
        grid.RowDefinitions.Add(new RowDefinition
            { Height = new GridLength(1, GridUnitType.Star) });

        TextBlock text = new()
        {
            Text = Component.Name,
        };
        text.SetResourceReference(StyleProperty, "MaterialDesignCaptionTextBlock");
        text.SetResourceReference(TextBlock.ForegroundProperty, "PrimaryHueDarkForegroundBrush");

        Binding b = new()
        {
            Source = DataContext,
            Path = new PropertyPath("ShowTitle"),
            Converter = new BoolToVisibilityConverter(),
            Mode = BindingMode.OneWay,
        };
        text.SetBinding(VisibilityProperty, b);

        Grid.SetRow(text, 0);

        Border border = new()
        {
            Background = (Brush)FindResource("PrimaryHueDarkForegroundBrush"),
            SnapsToDevicePixels = true,
            Height = 1,
            HorizontalAlignment = HorizontalAlignment.Stretch,
        };

        Binding b2 = new()
        {
            Source = DataContext,
            Path = new PropertyPath("ShowTitle"),
            Converter = new BoolToVisibilityConverter(),
            Mode = BindingMode.OneWay,
        };
        border.SetBinding(VisibilityProperty, b2);

        Grid.SetRow(border, 1);

        if (content != null)
            Grid.SetRow(content, 2);

        grid.Children.Add(text);
        grid.Children.Add(border);
        if (content != null)
            grid.Children.Add(content);

        return grid;
    }
}
