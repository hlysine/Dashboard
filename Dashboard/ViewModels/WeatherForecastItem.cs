using Dashboard.Services;
using Dashboard.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Dashboard.ViewModels
{
    public class WeatherForecastItem
    {
        private static readonly Dictionary<int, IconLabelPair> iconLabels = new()
        {
            [200] = new()
                { Label = "thunderstorm with light rain", Icon = "storm-showers" },
            [201] = new()
                { Label = "thunderstorm with rain", Icon = "storm-showers" },
            [202] = new()
                { Label = "thunderstorm with heavy rain", Icon = "storm-showers" },
            [210] = new()
                { Label = "light thunderstorm", Icon = "storm-showers" },
            [211] = new()
                { Label = "thunderstorm", Icon = "thunderstorm" },
            [212] = new()
                { Label = "heavy thunderstorm", Icon = "thunderstorm" },
            [221] = new()
                { Label = "ragged thunderstorm", Icon = "thunderstorm" },
            [230] = new()
                { Label = "thunderstorm with light drizzle", Icon = "storm-showers" },
            [231] = new()
                { Label = "thunderstorm with drizzle", Icon = "storm-showers" },
            [232] = new()
                { Label = "thunderstorm with heavy drizzle", Icon = "storm-showers" },
            [300] = new()
                { Label = "light intensity drizzle", Icon = "sprinkle" },
            [301] = new()
                { Label = "drizzle", Icon = "sprinkle" },
            [302] = new()
                { Label = "heavy intensity drizzle", Icon = "sprinkle" },
            [310] = new()
                { Label = "light intensity drizzle rain", Icon = "sprinkle" },
            [311] = new()
                { Label = "drizzle rain", Icon = "sprinkle" },
            [312] = new()
                { Label = "heavy intensity drizzle rain", Icon = "sprinkle" },
            [313] = new()
                { Label = "shower rain and drizzle", Icon = "sprinkle" },
            [314] = new()
                { Label = "heavy shower rain and drizzle", Icon = "sprinkle" },
            [321] = new()
                { Label = "shower drizzle", Icon = "sprinkle" },
            [500] = new()
                { Label = "light rain", Icon = "rain" },
            [501] = new()
                { Label = "moderate rain", Icon = "rain" },
            [502] = new()
                { Label = "heavy intensity rain", Icon = "rain" },
            [503] = new()
                { Label = "very heavy rain", Icon = "rain" },
            [504] = new()
                { Label = "extreme rain", Icon = "rain" },
            [511] = new()
                { Label = "freezing rain", Icon = "rain-mix" },
            [520] = new()
                { Label = "light intensity shower rain", Icon = "showers" },
            [521] = new()
                { Label = "shower rain", Icon = "showers" },
            [522] = new()
                { Label = "heavy intensity shower rain", Icon = "showers" },
            [531] = new()
                { Label = "ragged shower rain", Icon = "showers" },
            [600] = new()
                { Label = "light snow", Icon = "snow" },
            [601] = new()
                { Label = "snow", Icon = "snow" },
            [602] = new()
                { Label = "heavy snow", Icon = "snow" },
            [611] = new()
                { Label = "sleet", Icon = "sleet" },
            [612] = new()
                { Label = "shower sleet", Icon = "sleet" },
            [615] = new()
                { Label = "light rain and snow", Icon = "rain-mix" },
            [616] = new()
                { Label = "rain and snow", Icon = "rain-mix" },
            [620] = new()
                { Label = "light shower snow", Icon = "rain-mix" },
            [621] = new()
                { Label = "shower snow", Icon = "rain-mix" },
            [622] = new()
                { Label = "heavy shower snow", Icon = "rain-mix" },
            [701] = new()
                { Label = "mist", Icon = "sprinkle" },
            [711] = new()
                { Label = "smoke", Icon = "smoke" },
            [721] = new()
                { Label = "haze", Icon = "day-haze" },
            [731] = new()
                { Label = "sand, dust whirls", Icon = "cloudy-gusts" },
            [741] = new()
                { Label = "fog", Icon = "fog" },
            [751] = new()
                { Label = "sand", Icon = "cloudy-gusts" },
            [761] = new()
                { Label = "dust", Icon = "dust" },
            [762] = new()
                { Label = "volcanic ash", Icon = "smog" },
            [771] = new()
                { Label = "squalls", Icon = "day-windy" },
            [781] = new()
                { Label = "tornado", Icon = "tornado" },
            [800] = new()
                { Label = "clear sky", Icon = "sunny" },
            [801] = new()
                { Label = "few clouds", Icon = "cloudy" },
            [802] = new()
                { Label = "scattered clouds", Icon = "cloudy" },
            [803] = new()
                { Label = "broken clouds", Icon = "cloudy" },
            [804] = new()
                { Label = "overcast clouds", Icon = "cloudy" },
            [900] = new()
                { Label = "tornado", Icon = "tornado" },
            [901] = new()
                { Label = "tropical storm", Icon = "hurricane" },
            [902] = new()
                { Label = "hurricane", Icon = "hurricane" },
            [903] = new()
                { Label = "cold", Icon = "snowflake-cold" },
            [904] = new()
                { Label = "hot", Icon = "hot" },
            [905] = new()
                { Label = "windy", Icon = "windy" },
            [906] = new()
                { Label = "hail", Icon = "hail" },
            [951] = new()
                { Label = "calm", Icon = "sunny" },
            [952] = new()
                { Label = "light breeze", Icon = "cloudy-gusts" },
            [953] = new()
                { Label = "gentle breeze", Icon = "cloudy-gusts" },
            [954] = new()
                { Label = "moderate breeze", Icon = "cloudy-gusts" },
            [955] = new()
                { Label = "fresh breeze", Icon = "cloudy-gusts" },
            [956] = new()
                { Label = "strong breeze", Icon = "cloudy-gusts" },
            [957] = new()
                { Label = "high wind, near gale", Icon = "cloudy-gusts" },
            [958] = new()
                { Label = "gale", Icon = "cloudy-gusts" },
            [959] = new()
                { Label = "severe gale", Icon = "cloudy-gusts" },
            [960] = new()
                { Label = "storm", Icon = "thunderstorm" },
            [961] = new()
                { Label = "violent storm", Icon = "thunderstorm" },
            [962] = new()
                { Label = "hurricane", Icon = "cloudy-gusts" },
        };

        private ForecastItem forecast;

        private Units units;

        public DateTime DateTime { get => forecast.UtcDateTime.ToLocalTime(); }

        public bool ShowDate { get => DateTime.TimeOfDay < TimeSpan.FromHours(3); }

        public MainInfo MainInfo { get => forecast.Main; }

        public Rain Rain { get => forecast.Rain; }

        public string TemperatureUnit
        {
            get
            {
                if (units == Units.Standard) return "K";
                else if (units == Units.Metric) return "°C";
                else return "°F";
            }
        }

        public string IconUrl
        {
            get
            {
                var code = forecast.Weather.First().Id;
                var icon = iconLabels[code].Icon;

                if (!(code > 699 && code < 800) && !(code > 899 && code < 1000))
                    icon = "day-" + icon;

                return $"https://raw.githubusercontent.com/erikflowers/weather-icons/master/svg/wi-{icon}.svg";
            }
        }

        public WeatherForecastItem(ForecastItem _forecast, Units _units) => (forecast, units) = (_forecast, _units);
    }

    public class IconLabelPair
    {
        public string Label { get; set; }
        public string Icon { get; set; }
    }
}
