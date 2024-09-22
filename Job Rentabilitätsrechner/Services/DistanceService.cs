using Job_Rentabilitätsrechner.Interfaces;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json;
using Job_Rentabilitätsrechner.Models;
using System.Globalization;
using Job_Rentabilitätsrechner.Controller;
using System.Net;

namespace Job_Rentabilitätsrechner.Services
{


    public class DistanceService : IDistanceService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger<DistanceApiController> _logger;

        public DistanceService(HttpClient httpClient, IConfiguration configuration, ILogger<DistanceApiController> logger)
        {
            _httpClient = httpClient;
            _apiKey = configuration["OpenRouteServiceApiKey"];  // Hole API-Key aus appsettings.json
            _logger = logger;

            //proxy config
           
            var proxy = new WebProxy("http://winproxy.server.lan:3128", true);
            var handler = new HttpClientHandler
            {
                Proxy = proxy,
                UseProxy = true
            };

            _httpClient = new HttpClient(handler);
        }

        public async Task<double?> GetDistanceAsync((double Longitude, double Latitude) fromCoords, (double Longitude, double Latitude) toCoords)
        {
            var url = $"https://api.openrouteservice.org/v2/directions/driving-car?api_key={_apiKey}&start={fromCoords.Longitude.ToString(CultureInfo.InvariantCulture)},{fromCoords.Latitude.ToString(CultureInfo.InvariantCulture)}&end={toCoords.Longitude.ToString(CultureInfo.InvariantCulture)},{toCoords.Latitude.ToString(CultureInfo.InvariantCulture)}";
            var response = await _httpClient.GetStringAsync(url);
            _logger.LogError("API Response: " + response);

            var distanceResult = JsonConvert.DeserializeObject<DistanceResult>(response);

            if (distanceResult?.features != null && distanceResult.features.Length > 0)
            {
                var distance = distanceResult.features[0].properties.segments[0].distance / 1000.0;
                return distance;
            }
            else
            {
                _logger.LogError("Die Antwort enthält keine Routen.");
                return null;
            }


        }

        public async Task<RouteInfo?> GetDurationAsync((double Longitude, double Latitude) fromCoords, (double Longitude, double Latitude) toCoords)
        {
            var url = $"https://api.openrouteservice.org/v2/directions/driving-car?api_key={_apiKey}&start={fromCoords.Longitude.ToString(CultureInfo.InvariantCulture)},{fromCoords.Latitude.ToString(CultureInfo.InvariantCulture)}&end={toCoords.Longitude.ToString(CultureInfo.InvariantCulture)},{toCoords.Latitude.ToString(CultureInfo.InvariantCulture)}";
            var response = await _httpClient.GetStringAsync(url);
            _logger.LogError("API Response: " + response);

            var distanceResult = JsonConvert.DeserializeObject<DistanceResult>(response);

            if (distanceResult?.features != null && distanceResult.features.Length > 0)
            {

                var durationInMinutes = distanceResult.features[0].properties.segments[0].duration / 60.0;
                var durationSeconds = distanceResult.features[0].properties.segments[0].duration % 60;
                return new RouteInfo
                {
                    Duration = (int)durationInMinutes,
                    DurationSeconds = (int)durationSeconds
                };
            }
            else
            {
                _logger.LogError("Die Antwort enthält keine Routen.");
                return null;
            }
        }
    }
}
