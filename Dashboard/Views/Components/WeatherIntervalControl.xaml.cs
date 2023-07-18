using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Dashboard.ViewModels;
using Svg2Xaml;

namespace Dashboard.Views.Components
{
    /// <summary>
    /// Interaction logic for WeatherIntervalControl.xaml
    /// </summary>
    public partial class WeatherIntervalControl : UserControl
    {
        public WeatherIntervalControl()
        {
            InitializeComponent();
        }

        private async void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is WeatherForecastItem data)
            {
                WebClient wc = new WebClient();
                using MemoryStream stream = new MemoryStream(await wc.DownloadDataTaskAsync(new Uri(data.IconUrl)));
                imgWeather.OpacityMask = new ImageBrush(SvgReader.Load(stream)) { Stretch = Stretch.Uniform };
                imgWeather.SetResourceReference(Image.SourceProperty, "EmptyImageDrawing");
            }
        }
    }
}
