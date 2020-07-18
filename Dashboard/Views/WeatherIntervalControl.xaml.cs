using Dashboard.ViewModels;
using Svg2Xaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Dashboard.Views
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
                using (MemoryStream stream = new MemoryStream(await wc.DownloadDataTaskAsync(new Uri(data.IconUrl))))
                {
                    imgWeather.OpacityMask = new ImageBrush(SvgReader.Load(stream)) { Stretch = Stretch.Uniform };
                    imgWeather.Source = (DrawingImage)FindResource("EmptyImageDrawing");
                }
            }
        }
    }
}
