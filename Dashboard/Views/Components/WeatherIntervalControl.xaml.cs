using System;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Dashboard.ViewModels;
using Svg2Xaml;

namespace Dashboard.Views.Components;

/// <summary>
/// Interaction logic for WeatherIntervalControl.xaml
/// </summary>
public partial class WeatherIntervalControl : UserControl
{
    private readonly HttpClient httpClient = new();

    public WeatherIntervalControl()
    {
        InitializeComponent();
    }

    private async void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (DataContext is not WeatherForecastItem data)
            return;

        Stream netStream = await httpClient.GetStreamAsync(new Uri(data.IconUrl));
        imgWeather.OpacityMask = new ImageBrush(SvgReader.Load(netStream)) { Stretch = Stretch.Uniform };
        imgWeather.SetResourceReference(Image.SourceProperty, "EmptyImageDrawing");
    }
}
