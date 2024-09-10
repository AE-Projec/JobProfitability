using Job_Rentabilitätsrechner.Controller;
using Job_Rentabilitätsrechner.Interfaces;
using Job_Rentabilitätsrechner.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;

namespace Job_Rentabilitätsrechner.Pages
{
    public class CalculateModel : PageModel
    {
        private readonly IWearAndTearCalculator _wearAndTearCalculator;
        private readonly ICommuteCostCalculationService _commutCostCalculationService;
        private readonly INetSalaryCalculationService _netSalaryCalculationService;
        private readonly IFuelConsumptionAdjustment _fuelConsumptionAdjustment;
        private readonly IGeocodingService _geocodingService;
        private readonly IDistanceService _distanceService;


        #region BindProperties
        [BindProperty]
        public float CommuteDistance { get; set; }
        [BindProperty]
        public string? FuelPrice { get; set; }
        [BindProperty]
        public float DailyCost { get; set; }
        [BindProperty]
        public float FuelConsumption { get; set; } // Verbrauch in Litern pro 100 km   
        [BindProperty]
        public string? FuelType { get; set; }
        [BindProperty]
        public float GrossSalary { get; set; }
        [BindProperty]
        public float NewGrossSalary { get; set; }
        [BindProperty]
        public bool IncludeSoli { get; set; }
        [BindProperty]
        public int TaxClass { get; set; }
        [BindProperty]
        public bool ChurchTax { get; set; }
        [BindProperty]
        public float NetSalary { get; set; }
        [BindProperty]
        public float NewNetSalary { get; set; }
        [BindProperty]
        public int WearLevel { get; set; } = 2; // Standardwert für Fahrzeugabnutzung
        [BindProperty]
        public bool IncludeWearAndTear { get; set; }
        [BindProperty]
        public string? Bundesland { get; set; }
        [BindProperty]
        public string? TransmissionType { get; set; }
        [BindProperty]
        public int? GearCount { get; set; }
        [BindProperty]
        public float? ExternNetSalary { get; set; }
        [BindProperty]
        public int AverageCommuteDays { get; set; }
        [BindProperty]
        public float CommuteCostYear { get; set; }
        [BindProperty]
        public float WearAndTearYear { get; set; }
        [BindProperty]
        public bool UseExternalNetto { get; set; }
        [BindProperty]
        public bool UseCalculateDistance { get; set; }
        [BindProperty]
        public string? FromLocation { get; set; }
        [BindProperty]
        public string? ToLocation { get; set; }


        #endregion
        #region Public Variables
        public float TotalCost { get; set; }
        public float AdjustedSalary { get; set; }
        public float CommuteCost { get; set; }
        public float SalaryDifference { get; set; }
        public bool IsCalculated { get; set; }
        public float MonthlyWearAndTear { get; set; }
        public float TotalCostWearAndTear { get; set; }
        public float AdjustedNetSalary { get; set; }
        public float AdjustedNetYearSalary { get; set; }
        #endregion

        public JsonResult OnGetCalculateNetSalary(float grossSalary, float newGrossSalary, int taxClass, bool churchTax)
        {
            try
            {
                float kirchensteuerRate = GetChurchTaxRate(churchTax, Bundesland);
                float netSalary = _netSalaryCalculationService.CalculateNetSalary(grossSalary, taxClass, churchTax, kirchensteuerRate);
                float newNetSalary = _netSalaryCalculationService.CalculateNetSalary(newGrossSalary, taxClass, churchTax, kirchensteuerRate);
                return new JsonResult(new { NetSalary = netSalary, NewNetSalary = newNetSalary });
            }
            catch (Exception ex)
            {

                return new JsonResult(new { Error = ex.Message });
            }
        }
        public CalculateModel(IWearAndTearCalculator wearAndTearCalculator,
            ICommuteCostCalculationService commutCostCalculationService,
            INetSalaryCalculationService netSalaryCalculationService,
            IFuelConsumptionAdjustment fuelConsumptionAdjustment,
            IGeocodingService geocodingService,
            IDistanceService distanceService
            )
        {
            _wearAndTearCalculator = wearAndTearCalculator;
            _commutCostCalculationService = commutCostCalculationService;
            _netSalaryCalculationService = netSalaryCalculationService;
            _fuelConsumptionAdjustment = fuelConsumptionAdjustment;
            _geocodingService = geocodingService;
            _distanceService = distanceService;
           
        }

        public async Task<IActionResult> OnPostCalculateDistanceAsync()
        {
            if (UseCalculateDistance)
            {
                var url = $"/api/distanceApi/calculateDistance?fromLocation={FromLocation}&toLocation={ToLocation}";
                var client = new HttpClient();

                try
                {
                    var response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        var routeInfo = JsonConvert.DeserializeObject<dynamic>(result);

                        CommuteDistance = (float)(routeInfo.Distance * 2);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Fehler bei der Berechnung der Pendeldistanz.");
                    }
                }
                catch (Exception ex)
                {

                    ModelState.AddModelError("", $"Fehler: {ex.Message}");
                }
                

            }
            return Page();

        }
        
        public void OnPost()
        {

            // Berechnung der Kirchensteuer
            float kirchensteuerRate = GetChurchTaxRate(ChurchTax, Bundesland);

            // Berechnung des Nettogehalts (initiale Berechnung)
            float initialNetSalary = _netSalaryCalculationService.CalculateNetSalary(GrossSalary, TaxClass, ChurchTax, kirchensteuerRate);

            // Anpassung des Kraftstoffverbrauchs
            float adjustedFuelConsumption = _fuelConsumptionAdjustment.AdjustFuelConsumption(FuelConsumption, TransmissionType, GearCount);

            // Berechnung des Bruttoeinkommens nach Abzügen
            float bruttoNachAbzügen = _netSalaryCalculationService.CalculateGrossAfterDeductions(GrossSalary, TaxClass, ChurchTax, kirchensteuerRate);

            // Berechnung der Nettogehälter
            // Lokale Variablen für out-Parameter
            float netSalary;
            float newNetSalary;

            _netSalaryCalculationService.CalculateNetSalaries(GrossSalary, NewGrossSalary, TaxClass, ChurchTax, kirchensteuerRate, UseExternalNetto, ExternNetSalary, out netSalary, out newNetSalary);

            // Zuordnung der berechneten Werte zu den Properties
            NetSalary = netSalary;
            NewNetSalary = newNetSalary;

            // Überprüfung des Solidaritätszuschlags
            IncludeSoli = _netSalaryCalculationService.CheckSoliThreshold(GrossSalary, TaxClass);


            // Überprüfung der Pendeltage
            if (AverageCommuteDays < 0 || AverageCommuteDays > 7)
            {
                ModelState.AddModelError("AverageCommuteDays", "Die durchschnittliche Anzahl der Pendeltage pro Woche muss zwischen 0 und 7 liegen.");
                return;
            }


            // Lokale Variablen für Pendelkosten
            float commuteCost, commuteCostYear, totalCost, monthlyWearAndTear, totalCostWearAndTear, wearAndTearYear, adjustedNetSalary, salaryDifference, adjustedSalary, totalAnnualCost;

            // Berechnung der Pendelkosten und Gesamtkosten
            if (float.TryParse(FuelPrice, NumberStyles.Float, CultureInfo.InvariantCulture, out float parsedFuelPrice))
            {
                _commutCostCalculationService.CalculateCommuteAndTotalCosts(parsedFuelPrice, adjustedFuelConsumption, CommuteDistance, AverageCommuteDays,
                    WearLevel, IncludeWearAndTear, GrossSalary, NewGrossSalary, TaxClass, ChurchTax, kirchensteuerRate,
                    out commuteCost, out commuteCostYear, out totalCost, out monthlyWearAndTear, out totalCostWearAndTear,
                    out wearAndTearYear, out adjustedNetSalary, out salaryDifference, out adjustedSalary, out totalAnnualCost);

                //Nur Netto gehalt nach Radiobutton
                AdjustedNetYearSalary = Math.Max(0, NetSalary - totalAnnualCost);
                // Zuordnung der berechneten Werte zu den Properties
                CommuteCost = commuteCost;
                CommuteCostYear = commuteCostYear;
                TotalCost = totalCost;
                MonthlyWearAndTear = monthlyWearAndTear;
                TotalCostWearAndTear = totalCostWearAndTear;
                WearAndTearYear = wearAndTearYear;
                AdjustedNetSalary = adjustedNetSalary;
                SalaryDifference = salaryDifference;
                AdjustedSalary = adjustedSalary;
            }

            // Optional: Ausgabe in der View
            ViewData["BruttoNachAbzügen"] = bruttoNachAbzügen;



            IsCalculated = true;
        }

        //Hilfsmethode für Kirchensteuerrate
        private float GetChurchTaxRate(bool churchTax, string bundesland)
        {
            return _netSalaryCalculationService.CalculateChurchTaxRate(ChurchTax, Bundesland);
        }
    }
}