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
            
            var proxy = new WebProxy("your Proxy here", true);
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
            //_logger.LogError("API Response: " + response);

            var distanceResult = JsonConvert.DeserializeObject<DistanceResult>(response);

            if (distanceResult?.features != null && distanceResult.features.Length > 0)
            {
                var distance = distanceResult.features[0].properties.segments[0].distance / 1000.0;
                return distance;
            }
            else
            {
                return null;
            }


        }
        public async Task<RouteInfo?> GetDurationAsync((double Longitude, double Latitude) fromCoords, (double Longitude, double Latitude) toCoords)
        {
            var url = $"https://api.openrouteservice.org/v2/directions/driving-car?api_key={_apiKey}&start={fromCoords.Longitude.ToString(CultureInfo.InvariantCulture)},{fromCoords.Latitude.ToString(CultureInfo.InvariantCulture)}&end={toCoords.Longitude.ToString(CultureInfo.InvariantCulture)},{toCoords.Latitude.ToString(CultureInfo.InvariantCulture)}";
            var response = await _httpClient.GetStringAsync(url);

            var distanceResult = JsonConvert.DeserializeObject<DistanceResult>(response);

            if (distanceResult?.features != null && distanceResult.features.Length > 0)
            {
                var segment = distanceResult.features[0].properties.segments[0];
                var durationInMinutes = segment.duration / 60.0;
                var durationSeconds = segment.duration % 60;

                var routeInfo = new RouteInfo
                {
                    Duration = (int)durationInMinutes,
                    DurationSeconds = (int)durationSeconds
                };

                // Hier kannst du überprüfen, ob `OldDuration` und `OldDurationSeconds` ebenfalls verfügbar sind,
                // falls sie aus `distanceResult` extrahierbar sind:
                if (distanceResult.features.Length > 1) // Beispiel: Überprüfung auf andere Daten
                {
                    var oldSegment = distanceResult.features[1].properties.segments[0]; // Fiktive Extraktion
                    var oldDurationInMinutes = oldSegment.duration / 60.0;
                    var oldDurationSeconds = oldSegment.duration % 60;

                    routeInfo.OldDuration = (int)oldDurationInMinutes;
                    routeInfo.OldDurationSeconds = (int)oldDurationSeconds;
                }

                return routeInfo;
            }
            else
            {
                return null;
            }
        }




        /*
        public async Task<RouteInfo?> GetDurationAsync((double Longitude, double Latitude) fromCoords, (double Longitude, double Latitude) toCoords)
        {
            var url = $"https://api.openrouteservice.org/v2/directions/driving-car?api_key={_apiKey}&start={fromCoords.Longitude.ToString(CultureInfo.InvariantCulture)},{fromCoords.Latitude.ToString(CultureInfo.InvariantCulture)}&end={toCoords.Longitude.ToString(CultureInfo.InvariantCulture)},{toCoords.Latitude.ToString(CultureInfo.InvariantCulture)}";
            var response = await _httpClient.GetStringAsync(url);

            var distanceResult = JsonConvert.DeserializeObject<DistanceResult>(response);

            if (distanceResult?.features != null && distanceResult.features.Length > 0)
            {
                var segment = distanceResult.features[0].properties.segments[0];

                var durationInMinutes = segment.duration / 60.0;
                var durationSeconds = segment.duration % 60;

                return new RouteInfo
                {
                    Duration = (int)durationInMinutes,
                    DurationSeconds = (int)durationSeconds
                };
            }
            else
            {
                return null;
            }
        }*/
    }
}
