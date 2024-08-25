using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;

namespace Job_Rentabilitätsrechner.Pages
{
    public class CalculateModel : PageModel
    {
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
        public bool IncludeSoli {  get; set; }
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
        #endregion

        #region Public Variables
        public float TotalCost { get; set; }
        public float AdjustedSalary { get; set; }
        public float CommuteCost { get; set; }
        public float SalaryDifference { get; set; }
        public bool IsCalculated { get; set; }
        public float MonthlyWearAndTear { get; set; }
        public float TotalCostWearAnTear { get; set; }
        public float AdjustedNetSalary { get; set; }
        #endregion

        public JsonResult OnGetCalculateNetSalary(float grossSalary, float newGrossSalary, int taxClass, bool churchTax)
        {

            System.Diagnostics.Debug.WriteLine("CalculateNetSalary Handler Called");
            System.Diagnostics.Debug.WriteLine($"grossSalary: {grossSalary}, taxClass: {taxClass}, churchTax: {churchTax}");
            try
            {
                BruttoNettoCalculator calculator = new BruttoNettoCalculator();
                float netSalary = calculator.CalculateNetto(grossSalary, taxClass, churchTax);
                float newNetSalary = calculator.CalculateNetto(newGrossSalary, taxClass, churchTax);
                return new JsonResult(new { NetSalary = netSalary, NewNetSalary = newNetSalary });
            }
            catch (Exception ex)
            {

                return new JsonResult(new { Error = ex.Message });
            }
        }
        

        public void OnPost()
        {
            
            BruttoNettoCalculator calculator = new BruttoNettoCalculator();
            float annualWearAndTearCost = 0;

            NetSalary = calculator.CalculateNetto(GrossSalary, TaxClass, ChurchTax);
            NewNetSalary = calculator.CalculateNetto(NewGrossSalary, TaxClass, ChurchTax);

            float soliThreshold = (TaxClass == 3 || TaxClass == 4) ? 136826f : 68413f;
            if(GrossSalary > soliThreshold)
            {
                IncludeSoli = true;
            }

            // Berechnung der Kosten pro Kilometer
            if (float.TryParse(FuelPrice,NumberStyles.Float,CultureInfo.InvariantCulture,out float parsedFuelPrice))
            {
                
                float costPerKm = (FuelConsumption / 100f) * parsedFuelPrice;

               // DailyCost = CommuteDistance * costPerKm;

                // Berechnung der jährlichen Pendelkosten (Hin- und Rückfahrt, 230 Arbeitstage)
                float annualCommuteCost = CommuteDistance * 2 * costPerKm * 230;
               
                // Monatliche Pendelkosten
                CommuteCost = (float)Math.Round(annualCommuteCost / 12,2);

                if (IncludeWearAndTear)
                {
                    WearAndTearCalculator wearAndTearCalculator = new WearAndTearCalculator();
                    annualWearAndTearCost = wearAndTearCalculator.CalculateWearAndTear(CommuteDistance * 230, WearLevel);
                }
                

                // Gesamtjahreskosten (Pendelkosten + Abnutzungskosten)
                float totalAnnualCost = annualCommuteCost + annualWearAndTearCost;

                TotalCost = (float)Math.Round(totalAnnualCost / 12, 2);

                MonthlyWearAndTear = TotalCost - CommuteCost;

                TotalCostWearAnTear = totalAnnualCost;

                float adjustedGrossSalary = NewGrossSalary - totalAnnualCost;

                AdjustedNetSalary = calculator.CalculateNetto(adjustedGrossSalary,TaxClass,ChurchTax);

                // Berechne die Gehaltsdifferenz nach Pendelkosten (Neue Gehalt - Alte Gehalt)
                SalaryDifference = (NewGrossSalary - GrossSalary) - totalAnnualCost;

                // Berechne das angepasste Jahresgehalt (nach Abzug der Pendelkosten)
                AdjustedSalary = NewGrossSalary - totalAnnualCost;

                IsCalculated = true;
            }
 
        }
    }
}
