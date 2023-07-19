using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Dashboard.Components;
using Dashboard.Utilities;
using Dashboard.ViewModels;

namespace Dashboard.Views.Components;

/// <summary>
/// Interaction logic for WeatherView.xaml
/// </summary>
public partial class WeatherView : WeatherViewBase
{
    public WeatherView(WeatherComponent component = null) : base(component)
    {
        InitializeComponent();
        Load();
        Component.PropertyChanged += Component_PropertyChanged;
    }

    private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        imgTransform.X = -((ScrollViewer)sender).HorizontalOffset;
    }

    private void Component_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(WeatherComponent.Forecast) && listWeather.IsLoaded)
        {
            drawGraph();
        }
    }

    private void getTemperaturePoints(out Point[] temp, out Point[] feelsLike)
    {
        const double v_margin = 10;

        var scroll = VisualTreeHelpers.FindChild<ScrollViewer>(listWeather);
        double width = scroll.ExtentWidth;
        int count = listWeather.Items.Count;
        double itemWidth = width / count;

        double min = double.PositiveInfinity;
        double max = double.NegativeInfinity;
        foreach (WeatherForecastItem item in Component.Forecast)
        {
            min = Math.Min(item.MainInfo.Temperature, min);
            max = Math.Max(item.MainInfo.Temperature, max);

            min = Math.Min(item.MainInfo.FeelsLike, min);
            max = Math.Max(item.MainInfo.FeelsLike, max);
        }

        List<Point> points = new();
        List<Point> points2 = new();

        for (var i = 0; i < Component.Forecast.Count; i++)
        {
            double top = 1 - (Component.Forecast[i].MainInfo.Temperature - min) / (max - min);
            points.Add(new Point(itemWidth / 2 + i * itemWidth, (canvasTemperature.ActualHeight - v_margin * 2) * top + v_margin));

            double top2 = 1 - (Component.Forecast[i].MainInfo.FeelsLike - min) / (max - min);
            points2.Add(new Point(itemWidth / 2 + i * itemWidth, (canvasTemperature.ActualHeight - v_margin * 2) * top2 + v_margin));
        }

        temp = points.ToArray();
        feelsLike = points2.ToArray();
    }

    private void listWeather_Loaded(object sender, RoutedEventArgs e)
    {
        drawGraph();
    }

    private static PathGeometry getPath(Point[] points)
    {
        BezierSpline.GetCurveControlPoints(points, out Point[] cp1, out Point[] cp2);

        PathSegmentCollection lines = new();
        for (var i = 0; i < cp1.Length; ++i)
        {
            lines.Add(new BezierSegment(cp1[i], cp2[i], points[i + 1], true));
        }
        PathFigure f = new(points[0], lines, false);
        return new PathGeometry(new[] { f });
    }

    private void drawGraph()
    {
        getTemperaturePoints(out Point[] points, out Point[] points2);


        Path path = new() { StrokeThickness = 3, Data = getPath(points2), Opacity = 0.5d };
        path.SetResourceReference(Shape.StrokeProperty, "PrimaryHueDarkForegroundBrush");
        canvasTemperature.Children.Add(path);

        Path path2 = new() { StrokeThickness = 3, Data = getPath(points) };
        path2.SetResourceReference(Shape.StrokeProperty, "PrimaryHueDarkForegroundBrush");
        canvasTemperature.Children.Add(path2);


        for (var i = 0; i < points.Length; i++)
        {
            TextBlock text = new()
            {
                Text = Component.Forecast[i].MainInfo.Temperature.ToString("F0") + Component.Forecast[i].TemperatureUnit,
                Padding = new Thickness(2),
            };
            text.SetResourceReference(TextBlock.BackgroundProperty, "PrimaryHueDarkForegroundBrush");
            text.SetResourceReference(TextBlock.ForegroundProperty, "PrimaryHueDarkBrush");

            text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            text.Arrange(new Rect(text.DesiredSize));

            Canvas.SetLeft(text, points[i].X - text.ActualWidth / 2);
            Canvas.SetTop(text, points[i].Y - text.ActualHeight / 2);
            canvasTemperature.Children.Add(text);
        }
    }
}

public abstract class WeatherViewBase : DashboardView<WeatherComponent>
{
    protected WeatherViewBase(WeatherComponent component) : base(component)
    {

    }

    protected WeatherViewBase() : this(null)
    {

    }
}
