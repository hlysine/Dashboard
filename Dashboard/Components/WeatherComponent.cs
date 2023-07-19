using Dashboard.Config;
using Dashboard.ViewModels;
using Dashboard.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dashboard.Components;

public class WeatherComponent : AutoRefreshComponent
{
    protected override string DefaultName => "Weather";

    [RequireService(nameof(OpenWeatherMapServiceId))]
    public OpenWeatherMapService OpenWeatherMap { get; set; }

    [PersistentConfig]
    public string OpenWeatherMapServiceId { get; set; }

    [PersistentConfig]
    public Units Units { get; set; } = Units.Metric;

    public override TimeSpan ForegroundRefreshRate => TimeSpan.FromMinutes(30);
    public override TimeSpan BackgroundRefreshRate => TimeSpan.FromHours(1);

    private List<WeatherForecastItem> forecast = new();

    public List<WeatherForecastItem> Forecast
    {
        get => forecast;
        set => SetAndNotify(ref forecast, value);
    }

    private async Task loadForecast()
    {
        ForecastResponse response = await OpenWeatherMap.GetDailyForecast(Units);
        Forecast.Clear();
        Forecast.AddRange(response.List.Select(x => new WeatherForecastItem(x, Units)));
        NotifyChanged(nameof(Forecast));
    }

    protected override async void OnInitializeSelf()
    {
        if (OpenWeatherMap.CanAuthorize)
        {
            if (OpenWeatherMap.IsAuthorized)
            {
                await loadForecast();
                StartAutoRefresh();
            }
        }
        Loaded = true;
    }

    protected override void OnInitializeDependencies()
    {
    }

    protected override async void OnRefresh()
    {
        await loadForecast();
    }
}
