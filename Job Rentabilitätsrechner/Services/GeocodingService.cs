using Job_Rentabilitätsrechner.Interfaces;
using Job_Rentabilitätsrechner.Models;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Newtonsoft.Json;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Job_Rentabilitätsrechner.Services
{

    public class GeocodingService : IGeocodingService
    {
        private readonly HttpClient _httpClient;
        private readonly string? _apiKey;

        public GeocodingService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["OpenRouteServiceApiKey"];

            //proxy config
            
            var proxy = new WebProxy("http://winproxy.server.lan:3128", true);
            var handler = new HttpClientHandler
            {
                Proxy = proxy,
                UseProxy = true
            };

            _httpClient = new HttpClient(handler);

        }

        public async Task<(double Longitude, double Latitude)?> GetCoordinatesAsync(string address)
        {
            var url = $"https://api.openrouteservice.org/geocode/search?api_key={_apiKey}&text={address}";
            var response = await _httpClient.GetStringAsync(url);
            var geocodeResult = JsonConvert.DeserializeObject<GeocodeResult>(response);

            if (geocodeResult.features.Length > 0)
            {
                var coords = geocodeResult.features[0].geometry.coordinates;
                return (coords[0], coords[1]);
            }
            return null;
        }
    }


}