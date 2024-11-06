using Job_Rentabilitätsrechner.Interfaces;
using Job_Rentabilitätsrechner.Models;
using Job_Rentabilitätsrechner.Services;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Globalization;

namespace Job_Rentabilitätsrechner.Controller
{
    public static class DistanceData
    {
        //neuer job
        public static float Distance { get; set; }
        public static float FullDistance { get; set; }

        //alter job
        public static float OldDistance { get; set; }
        public static float OldFullDistance { get; set; }

    }

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
                // _logger.LogInformation("Berechnung der Distanz zwischen {FromLocation} und {ToLocation}", fromLocation, toLocation);

                // 1. Koordinaten für Startadresse abrufen
                var fromCoords = await _geocodingService.GetCoordinatesAsync(fromLocation);
                if (fromCoords == null)
                {
                    // _logger.LogError("Startadresse {FromLocation} nicht gefunden.", fromLocation);
                    return BadRequest("Startadresse nicht gefunden.");
                }

                // 2. Koordinaten für Zieladresse abrufen
                var toCoords = await _geocodingService.GetCoordinatesAsync(toLocation);
                if (toCoords == null)
                {
                    // _logger.LogError("Zieladresse {ToLocation} nicht gefunden.", toLocation);
                    return BadRequest("Zieladresse nicht gefunden.");
                }

                // 3. Distanz zwischen den beiden Koordinaten berechnen
                var distance = await _distanceService.GetDistanceAsync(fromCoords.Value, toCoords.Value);
                var duration = await _distanceService.GetDurationAsync(fromCoords.Value, toCoords.Value);
                DistanceData.FullDistance = (float)Math.Round((float)(distance * 2), 2);
                DistanceData.Distance = (float)distance;

                if (distance == null)
                {
                    //_logger.LogError("Fehler bei der Berechnung der Distanz zwischen {FromLocation} und {ToLocation}.", fromLocation, toLocation);
                    return BadRequest("Fehler bei der Berechnung der Distanz.");
                }

                // _logger.LogInformation("Die berechnete Distanz zwischen {FromLocation} und {ToLocation} beträgt {Distance} km", fromLocation, toLocation, distance);
                // _logger.LogInformation("Die berechnete Distanz zwischen {FromLocation} und {ToLocation} und zurück beträgt {Distance} km", fromLocation, toLocation, DistanceData.FullDistance);
                // _logger.LogInformation("Die berechnete Duration zwischen {FromLocation} und {ToLocation} beträgt {duration.Duration} min und {duration.DurationSeconds} in sekunden", fromLocation, toLocation, duration.Duration, duration.DurationSeconds);
                return Ok(new
                {
                    Distance = DistanceData.Distance,
                    duration.Duration,
                    duration.DurationSeconds,
                    FullDistance = DistanceData.FullDistance
                });

            }
            catch (HttpRequestException ex)
            {
                // _logger.LogError(ex, "Fehler bei der Anfrage an die API.");
                return StatusCode(500, $"Fehler bei der API-Anfrage: {ex.Message}");
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Ein unerwarteter Fehler ist aufgetreten.");
                return StatusCode(500, $"Ein unerwarteter Fehler ist aufgetreten.: {ex.Message}");
            }
        }

        [HttpGet("oldCalculateDistance")]
        public async Task<IActionResult> OldCalculateDistance(string fromLocation, string toLocation)
        {
            try
            {
                // Log für die Berechnung der Distanz zwischen den alten Arbeitsorten
                _logger.LogInformation("Berechnung der Distanz zwischen {FromOldLocation} und {ToOldLocation}", fromLocation, toLocation);

                // 1. Koordinaten für die alte Startadresse abrufen
                var fromOldCoords = await _geocodingService.GetCoordinatesAsync(fromLocation);
                if (fromOldCoords == null)
                {
                    _logger.LogError("Startadresse {FromOldLocation} nicht gefunden.", fromLocation);
                    return BadRequest("Startadresse nicht gefunden.");
                }

                // 2. Koordinaten für die alte Zieladresse abrufen
                var toOldCoords = await _geocodingService.GetCoordinatesAsync(toLocation);
                if (toOldCoords == null)
                {
                    _logger.LogError("Zieladresse {ToOldLocation} nicht gefunden.", toLocation);
                    return BadRequest("Zieladresse nicht gefunden.");
                }

                // 3. Distanz und Dauer für die alten Arbeitsorte berechnen
                var oldDistance = await _distanceService.GetDistanceAsync(fromOldCoords.Value, toOldCoords.Value);
                var oldDuration = await _distanceService.GetDurationAsync(fromOldCoords.Value, toOldCoords.Value);
                // Speichern der alten Distanzen in DistanceData
                DistanceData.OldFullDistance = (float)Math.Round((float)(oldDistance * 2), 2);
                DistanceData.OldDistance = (float)oldDistance;

                if (oldDistance == null)
                {
                    _logger.LogError("Fehler bei der Berechnung der alten Distanz zwischen {FromOldLocation} und {ToOldLocation}.", fromLocation, toLocation);
                    return BadRequest("Fehler bei der Berechnung der Distanz.");
                }

                // Log für die berechnete Distanz und Dauer
               // _logger.LogInformation("Die berechnete Distanz zwischen {FromOldLocation} und {ToOldLocation} beträgt {Distance} km", fromLocation, toLocation, DistanceData.OldDistance);
               // _logger.LogInformation("Die berechnete Distanz zwischen {FromOldLocation} und {ToOldLocation} und zurück beträgt {Distance} km", fromLocation, toLocation, DistanceData.OldFullDistance);
               // _logger.LogInformation("Die berechnete Dauer zwischen {FromOldLocation} und {ToOldLocation} beträgt {duration} min und {durationSeconds} sek.", fromLocation, toLocation, oldDuration.Duration, oldDuration.DurationSeconds);

                // Rückgabe der berechneten Werte
                return Ok(new
                {
                    OldDistance = DistanceData.OldDistance,
                    OldFullDistance = DistanceData.OldFullDistance,
                    OldDuration = oldDuration.Duration,
                    OldDurationSeconds = oldDuration.DurationSeconds
                });

            }
            catch (HttpRequestException ex)
            {
                // _logger.LogError(ex, "Fehler bei der Anfrage an die API.");
                return StatusCode(500, $"Fehler bei der API-Anfrage: {ex.Message}");
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Ein unerwarteter Fehler ist aufgetreten.");
                return StatusCode(500, $"Ein unerwarteter Fehler ist aufgetreten.: {ex.Message}");
            }
        }
    }
}

