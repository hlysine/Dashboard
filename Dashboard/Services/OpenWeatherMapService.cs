using Dashboard.Config;
using Dashboard.Utilities;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard.Services
{
    public class OpenWeatherMapService : APIKeyService
    {
        [RequireService(nameof(LocationServiceId))]
        private LocationService Location { get; set; }
        [PersistentConfig]
        public string LocationServiceId { get; set; }

        public override bool IsAuthorized => client != null;

        private IRestClient client;

        public OpenWeatherMapService()
        {
        }

        protected override void OnInitialized()
        {
            client = new RestClient("https://api.openweathermap.org/").UseNewtonsoftJson();
            client.AddDefaultQueryParameter("appid", ApiKey);
        }

        public async Task<ForecastResponse> GetDailyForecast(Units units)
        {
            LocationResponse location = await Location.GetLocation();
            RestRequest request = new RestRequest("data/2.5/forecast", Method.GET);
            request.AddParameter("lon", location.Longitude);
            request.AddParameter("lat", location.Latitude);
            if (units == Units.Metric)
                request.AddParameter("units", "metric");
            else if (units == Units.Imperial)
                request.AddParameter("units", "imperial");
            IRestResponse<ForecastResponse> response = await client.ExecuteAsync<ForecastResponse>(request);
            return response.Data;
        }
    }

    public enum Units
    {
        Standard,
        Metric,
        Imperial
    }

    public class MainInfo
    {

        [JsonProperty("temp")]
        public double Temperature { get; set; }

        [JsonProperty("feels_like")]
        public double FeelsLike { get; set; }

        [JsonProperty("temp_min")]
        public double MinimumTemperature { get; set; }

        [JsonProperty("temp_max")]
        public double MaximumTemperature { get; set; }

        [JsonProperty("pressure")]
        public int Pressure { get; set; }

        [JsonProperty("sea_level")]
        public int SeaLevel { get; set; }

        [JsonProperty("grnd_level")]
        public int GroundLevel { get; set; }

        [JsonProperty("humidity")]
        public int Humidity { get; set; }

        [JsonProperty("temp_kf")]
        public double TemperatureKf { get; set; }

    }

    public class Weather
    {

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("main")]
        public string Main { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

    }

    public class Clouds
    {

        [JsonProperty("all")]
        public int All { get; set; }

    }

    public class Wind
    {

        [JsonProperty("speed")]
        public double Speed { get; set; }

        [JsonProperty("deg")]
        public int Degree { get; set; }

    }

    public class System
    {

        [JsonProperty("pod")]
        public string Pod { get; set; }

    }

    public class Rain
    {

        [JsonProperty("3h")]
        public double ThreeHours { get; set; }

    }

    public class ForecastItem
    {

        [JsonProperty("dt")]
        public long UtcSeconds { get; set; }

        [JsonProperty("main")]
        public MainInfo Main { get; set; }

        [JsonProperty("weather")]
        public List<Weather> Weather { get; set; }

        [JsonProperty("clouds")]
        public Clouds Clouds { get; set; }

        [JsonProperty("wind")]
        public Wind Wind { get; set; }

        [JsonProperty("sys")]
        public System System { get; set; }

        [JsonProperty("dt_txt")]
        public DateTime UtcDateTime { get; set; }

        [JsonProperty("rain")]
        public Rain Rain { get; set; }

    }

    public class Coordinates
    {

        [JsonProperty("lat")]
        public double Latitude { get; set; }

        [JsonProperty("lon")]
        public double Longitude { get; set; }

    }

    public class City
    {

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("coord")]
        public Coordinates Coordinates { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("population")]
        public int Population { get; set; }

        [JsonProperty("timezone")]
        public int Timezone { get; set; }

        [JsonProperty("sunrise")]
        public int Sunrise { get; set; }

        [JsonProperty("sunset")]
        public int Sunset { get; set; }

    }

    public class ForecastResponse
    {

        [JsonProperty("cod")]
        public string Code { get; set; }

        [JsonProperty("message")]
        public int Message { get; set; }

        [JsonProperty("cnt")]
        public int Count { get; set; }

        [JsonProperty("list")]
        public List<ForecastItem> List { get; set; }

        [JsonProperty("city")]
        public City City { get; set; }

    }
}
