using Dashboard.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dashboard.ViewModels
{
    public class WeatherForecastItem
    {
        private static readonly Dictionary<int, IconLabelPair> iconLabels = new()
        {
            [200] = new IconLabelPair
                { Label = "thunderstorm with light rain", Icon = "storm-showers" },
            [201] = new IconLabelPair
                { Label = "thunderstorm with rain", Icon = "storm-showers" },
            [202] = new IconLabelPair
                { Label = "thunderstorm with heavy rain", Icon = "storm-showers" },
            [210] = new IconLabelPair
                { Label = "light thunderstorm", Icon = "storm-showers" },
            [211] = new IconLabelPair
                { Label = "thunderstorm", Icon = "thunderstorm" },
            [212] = new IconLabelPair
                { Label = "heavy thunderstorm", Icon = "thunderstorm" },
            [221] = new IconLabelPair
                { Label = "ragged thunderstorm", Icon = "thunderstorm" },
            [230] = new IconLabelPair
                { Label = "thunderstorm with light drizzle", Icon = "storm-showers" },
            [231] = new IconLabelPair
                { Label = "thunderstorm with drizzle", Icon = "storm-showers" },
            [232] = new IconLabelPair
                { Label = "thunderstorm with heavy drizzle", Icon = "storm-showers" },
            [300] = new IconLabelPair
                { Label = "light intensity drizzle", Icon = "sprinkle" },
            [301] = new IconLabelPair
                { Label = "drizzle", Icon = "sprinkle" },
            [302] = new IconLabelPair
                { Label = "heavy intensity drizzle", Icon = "sprinkle" },
            [310] = new IconLabelPair
                { Label = "light intensity drizzle rain", Icon = "sprinkle" },
            [311] = new IconLabelPair
                { Label = "drizzle rain", Icon = "sprinkle" },
            [312] = new IconLabelPair
                { Label = "heavy intensity drizzle rain", Icon = "sprinkle" },
            [313] = new IconLabelPair
                { Label = "shower rain and drizzle", Icon = "sprinkle" },
            [314] = new IconLabelPair
                { Label = "heavy shower rain and drizzle", Icon = "sprinkle" },
            [321] = new IconLabelPair
                { Label = "shower drizzle", Icon = "sprinkle" },
            [500] = new IconLabelPair
                { Label = "light rain", Icon = "rain" },
            [501] = new IconLabelPair
                { Label = "moderate rain", Icon = "rain" },
            [502] = new IconLabelPair
                { Label = "heavy intensity rain", Icon = "rain" },
            [503] = new IconLabelPair
                { Label = "very heavy rain", Icon = "rain" },
            [504] = new IconLabelPair
                { Label = "extreme rain", Icon = "rain" },
            [511] = new IconLabelPair
                { Label = "freezing rain", Icon = "rain-mix" },
            [520] = new IconLabelPair
                { Label = "light intensity shower rain", Icon = "showers" },
            [521] = new IconLabelPair
                { Label = "shower rain", Icon = "showers" },
            [522] = new IconLabelPair
                { Label = "heavy intensity shower rain", Icon = "showers" },
            [531] = new IconLabelPair
                { Label = "ragged shower rain", Icon = "showers" },
            [600] = new IconLabelPair
                { Label = "light snow", Icon = "snow" },
            [601] = new IconLabelPair
                { Label = "snow", Icon = "snow" },
            [602] = new IconLabelPair
                { Label = "heavy snow", Icon = "snow" },
            [611] = new IconLabelPair
                { Label = "sleet", Icon = "sleet" },
            [612] = new IconLabelPair
                { Label = "shower sleet", Icon = "sleet" },
            [615] = new IconLabelPair
                { Label = "light rain and snow", Icon = "rain-mix" },
            [616] = new IconLabelPair
                { Label = "rain and snow", Icon = "rain-mix" },
            [620] = new IconLabelPair
                { Label = "light shower snow", Icon = "rain-mix" },
            [621] = new IconLabelPair
                { Label = "shower snow", Icon = "rain-mix" },
            [622] = new IconLabelPair
                { Label = "heavy shower snow", Icon = "rain-mix" },
            [701] = new IconLabelPair
                { Label = "mist", Icon = "sprinkle" },
            [711] = new IconLabelPair
                { Label = "smoke", Icon = "smoke" },
            [721] = new IconLabelPair
                { Label = "haze", Icon = "day-haze" },
            [731] = new IconLabelPair
                { Label = "sand, dust whirls", Icon = "cloudy-gusts" },
            [741] = new IconLabelPair
                { Label = "fog", Icon = "fog" },
            [751] = new IconLabelPair
                { Label = "sand", Icon = "cloudy-gusts" },
            [761] = new IconLabelPair
                { Label = "dust", Icon = "dust" },
            [762] = new IconLabelPair
                { Label = "volcanic ash", Icon = "smog" },
            [771] = new IconLabelPair
                { Label = "squalls", Icon = "day-windy" },
            [781] = new IconLabelPair
                { Label = "tornado", Icon = "tornado" },
            [800] = new IconLabelPair
                { Label = "clear sky", Icon = "sunny" },
            [801] = new IconLabelPair
                { Label = "few clouds", Icon = "cloudy" },
            [802] = new IconLabelPair
                { Label = "scattered clouds", Icon = "cloudy" },
            [803] = new IconLabelPair
                { Label = "broken clouds", Icon = "cloudy" },
            [804] = new IconLabelPair
                { Label = "overcast clouds", Icon = "cloudy" },
            [900] = new IconLabelPair
                { Label = "tornado", Icon = "tornado" },
            [901] = new IconLabelPair
                { Label = "tropical storm", Icon = "hurricane" },
            [902] = new IconLabelPair
                { Label = "hurricane", Icon = "hurricane" },
            [903] = new IconLabelPair
                { Label = "cold", Icon = "snowflake-cold" },
            [904] = new IconLabelPair
                { Label = "hot", Icon = "hot" },
            [905] = new IconLabelPair
                { Label = "windy", Icon = "windy" },
            [906] = new IconLabelPair
                { Label = "hail", Icon = "hail" },
            [951] = new IconLabelPair
                { Label = "calm", Icon = "sunny" },
            [952] = new IconLabelPair
                { Label = "light breeze", Icon = "cloudy-gusts" },
            [953] = new IconLabelPair
                { Label = "gentle breeze", Icon = "cloudy-gusts" },
            [954] = new IconLabelPair
                { Label = "moderate breeze", Icon = "cloudy-gusts" },
            [955] = new IconLabelPair
                { Label = "fresh breeze", Icon = "cloudy-gusts" },
            [956] = new IconLabelPair
                { Label = "strong breeze", Icon = "cloudy-gusts" },
            [957] = new IconLabelPair
                { Label = "high wind, near gale", Icon = "cloudy-gusts" },
            [958] = new IconLabelPair
                { Label = "gale", Icon = "cloudy-gusts" },
            [959] = new IconLabelPair
                { Label = "severe gale", Icon = "cloudy-gusts" },
            [960] = new IconLabelPair
                { Label = "storm", Icon = "thunderstorm" },
            [961] = new IconLabelPair
                { Label = "violent storm", Icon = "thunderstorm" },
            [962] = new IconLabelPair
                { Label = "hurricane", Icon = "cloudy-gusts" },
        };

        private ForecastItem forecast;

        private Units units;

        public DateTime DateTime => forecast.UtcDateTime.ToLocalTime();

        public bool ShowDate => DateTime.TimeOfDay < TimeSpan.FromHours(3);

        public MainInfo MainInfo => forecast.Main;

        public Rain Rain => forecast.Rain;

        public string TemperatureUnit
        {
            get
            {
                return units switch
                {
                    Units.Standard => "K",
                    Units.Metric => "°C",
                    _ => "°F"
                };
            }
        }

        public string IconUrl
        {
            get
            {
                var code = forecast.Weather.First().Id;
                var icon = iconLabels[code].Icon;

                if (code is (<= 699 or >= 800) and (<= 899 or >= 1000))
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