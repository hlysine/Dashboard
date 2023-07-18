using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using System.Threading.Tasks;

namespace Dashboard.Services;

public class LocationService : Service
{
    public override bool CanAuthorize => true;
    public override bool IsAuthorized => true;

    readonly IRestClient client = new RestClient("http://ip-api.com/", configureSerialization: s => s.UseNewtonsoftJson());

    public async Task<LocationResponse> GetLocation(string ipAddress = null)
    {
        var request = new RestRequest("json/{ip}", Method.Get);
        if (ipAddress != null)
            request.AddUrlSegment("ip", ipAddress);
        return (await client.ExecuteAsync<LocationResponse>(request)).Data;
    }
}

public class LocationResponse
{
    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("country")]
    public string Country { get; set; }

    [JsonProperty("countryCode")]
    public string CountryCode { get; set; }

    [JsonProperty("region")]
    public string Region { get; set; }

    [JsonProperty("regionName")]
    public string RegionName { get; set; }

    [JsonProperty("city")]
    public string City { get; set; }

    [JsonProperty("zip")]
    public string ZipCode { get; set; }

    [JsonProperty("lat")]
    public double Latitude { get; set; }

    [JsonProperty("lon")]
    public double Longitude { get; set; }

    [JsonProperty("timezone")]
    public string Timezone { get; set; }

    [JsonProperty("isp")]
    public string Isp { get; set; }

    [JsonProperty("org")]
    public string Organization { get; set; }

    [JsonProperty("as")]
    public string As { get; set; }

    [JsonProperty("query")]
    public string QueryIp { get; set; }
}