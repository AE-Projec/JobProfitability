using Job_Rentabilitätsrechner.Interfaces;
using Job_Rentabilitätsrechner.Models;
using Job_Rentabilitätsrechner.Services;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Job_Rentabilitätsrechner.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class DistanceApiController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly IGeocodingService _geocodingService;
        private readonly IDistanceService _distanceService;

        private readonly ILogger<DistanceApiController> _logger;

        public DistanceApiController(HttpClient httpClient, IConfiguration configuration,
            IGeocodingService geocodingService, IDistanceService distanceService,

            ILogger<DistanceApiController> logger)
        {
            _httpClient = httpClient;
            _apiKey = configuration["OpenRouteServiceApiKey"]; // API-Key aus appsettings.json laden
            _geocodingService = geocodingService;
            _distanceService = distanceService;

            _logger = logger;
        }

        [HttpGet("calculateDistance")]
        public async Task<IActionResult> CalculateDistance(string fromLocation, string toLocation)
        {
            try
            {
                _logger.LogInformation("Berechnung der Distanz zwischen {FromLocation} und {ToLocation}", fromLocation, toLocation);

                // 1. Koordinaten für Startadresse abrufen
                var fromCoords = await _geocodingService.GetCoordinatesAsync(fromLocation);
                if (fromCoords == null)
                {
                    _logger.LogError("Startadresse {FromLocation} nicht gefunden.", fromLocation);
                    return BadRequest("Startadresse nicht gefunden.");
                }

                // 2. Koordinaten für Zieladresse abrufen
                var toCoords = await _geocodingService.GetCoordinatesAsync(toLocation);
                if (toCoords == null)
                {
                    _logger.LogError("Zieladresse {ToLocation} nicht gefunden.", toLocation);
                    return BadRequest("Zieladresse nicht gefunden.");
                }

                // 3. Distanz zwischen den beiden Koordinaten berechnen
                var distance = await _distanceService.GetDistanceAsync(fromCoords.Value, toCoords.Value);
                var duration = await _distanceService.GetDurationAsync(fromCoords.Value, toCoords.Value);
                if (distance == null)
                {
                    _logger.LogError("Fehler bei der Berechnung der Distanz zwischen {FromLocation} und {ToLocation}.", fromLocation, toLocation);
                    return BadRequest("Fehler bei der Berechnung der Distanz.");
                }

                _logger.LogInformation("Die berechnete Distanz zwischen {FromLocation} und {ToLocation} beträgt {Distance} km", fromLocation, toLocation, distance);
                _logger.LogInformation("Die berechnete Duration zwischen {FromLocation} und {ToLocation} beträgt {duration.Duration} min und {duration.DurationSeconds} in sekunden", fromLocation, toLocation, duration.Duration, duration.DurationSeconds);
                return Ok(new { Distance = distance, duration.Duration, duration.DurationSeconds });
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Fehler bei der Anfrage an die API.");
                return StatusCode(500, $"Fehler bei der API-Anfrage: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ein unerwarteter Fehler ist aufgetreten.");
                return StatusCode(500, "Ein unerwarteter Fehler ist aufgetreten.");
            }
        }
    }
}
