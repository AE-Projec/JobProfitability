using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;

namespace Job_Rentabilitätsrechner.Pages
{
    public class CalculateModel : PageModel
    {
        [BindProperty]
        public float OldSalary { get; set; }

        [BindProperty]
        public float NewSalary { get; set; }

        [BindProperty]
        public float CommuteDistance { get; set; }

        [BindProperty]
        public string? FuelPrice { get; set; }

        [BindProperty]
        public float DailyCost { get; set; }

        [BindProperty]
        public float FuelConsumption { get; set; } // Verbrauch in Litern pro 100 km

        public float AdjustedSalary { get; set; }

        public float CommuteCost { get; set; }
        public float SalaryDifference { get; set; }
        public bool IsCalculated { get; set; }
        [BindProperty]
        public string? FuelType { get; set; }
        

        [BindProperty]
        public float GrossSalary { get; set; }

        [BindProperty]
        public int TaxClass { get; set; }

        [BindProperty]
        public bool ChurchTax { get; set; }

        public float NetSalary { get; set; }


        public void OnPost()
        {

           
            // Berechnung des Nettogehalts basierend auf dem Bruttogehalt, der Steuerklasse und der Kirchensteuer
            BruttoNettoCalculator calculator = new BruttoNettoCalculator();
            NetSalary = calculator.CalculateNetto(GrossSalary, TaxClass, ChurchTax);
            
            // Berechnung der Kosten pro Kilometer
            if (float.TryParse(FuelPrice,NumberStyles.Float,CultureInfo.InvariantCulture,out float parsedFuelPrice))
            {
                float costPerKm = (FuelConsumption / 100f) * parsedFuelPrice;
                System.Diagnostics.Debug.WriteLine("FuelConsumption " + FuelConsumption);
                System.Diagnostics.Debug.WriteLine("FuelPrice " + FuelPrice);
                System.Diagnostics.Debug.WriteLine("costperkm " + costPerKm);

                DailyCost = CommuteDistance * costPerKm;
                // Berechnung der jährlichen Pendelkosten (Hin- und Rückfahrt, 230 Arbeitstage)
                float annualCommuteCost = CommuteDistance * 2 * costPerKm * 230;
                
                

                // Monatliche Pendelkosten
                CommuteCost = (float)Math.Round(annualCommuteCost / 12,2);

                // Berechne die Gehaltsdifferenz nach Pendelkosten (Neue Gehalt - Alte Gehalt)
                SalaryDifference = (NewSalary - OldSalary) - annualCommuteCost;

                // Berechne das angepasste Jahresgehalt (nach Abzug der Pendelkosten)
                AdjustedSalary = NewSalary - annualCommuteCost;

                IsCalculated = true;
            }
 
        }
    }
}
