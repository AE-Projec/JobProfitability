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

            // Berechnung der Kirchensteuer
            float kirchensteuerRate = CalculateKirchensteuerRate();

            // Anpassung des Kraftstoffverbrauchs
            float adjustedFuelConsumption = AdjustFuelConsumption(FuelConsumption);

            // Berechnung des Bruttoeinkommens nach Abzügen
            float bruttoNachAbzügen = calculator.CalculateGrossAfterDeductions(GrossSalary, TaxClass, ChurchTax, kirchensteuerRate);


            // Berechnung der Nettogehälter
            CalculateNetSalaries(calculator, kirchensteuerRate);

            // Überprüfung des Solidaritätszuschlags
            CheckSoliThreshold(GrossSalary, TaxClass);

            // Berechnung der Pendelkosten und Gesamtkosten
            if (float.TryParse(FuelPrice, NumberStyles.Float, CultureInfo.InvariantCulture, out float parsedFuelPrice))
            {
                CalculateCommuteAndTotalCosts(parsedFuelPrice, adjustedFuelConsumption, calculator, kirchensteuerRate);
            }
            if (AverageCommuteDays < 0 || AverageCommuteDays > 7)
            {
                ModelState.AddModelError("AverageCommuteDays", "Die durchschnittliche Anzahl der Pendeltage pro Woche muss zwischen 0 und 7 liegen.");
                return;
            }
            // Optional: Ausgabe in der View
            ViewData["BruttoNachAbzügen"] = bruttoNachAbzügen;
        }





        private float CalculateKirchensteuerRate()
        {
            float rate = 0.09f;
            if (ChurchTax)
            {
                if (Bundesland == "bayern" || Bundesland == "bawue")
                {
                    rate = 0.08f;
                }
            }
            return rate;
        }

        private float AdjustFuelConsumption(float consumption)
        {
            float adjustedConsumption = consumption;

            if (TransmissionType == "automatic")
            {
                adjustedConsumption *= 1.10f; // Erhöhung um 10% bei Automatikgetriebe

                if (GearCount.HasValue && GearCount > 5)
                {
                    adjustedConsumption *= 0.95f; // Reduzierung um 5% bei mehr als 5 Gängen
                }
            }

            return adjustedConsumption;
        }

        private void CalculateNetSalaries(BruttoNettoCalculator calculator, float kirchensteuerRate)
        {

            if (UseExternalNetto && ExternNetSalary.HasValue && ExternNetSalary > 0)
            {
                NetSalary = ExternNetSalary.Value;


                float soli = calculator.CalculateSoliForExternalNetto(NetSalary, TaxClass);
                NetSalary -= soli;
            }
            else
            {
                NetSalary = calculator.CalculateNetto(GrossSalary, TaxClass, ChurchTax, kirchensteuerRate);
            }


            NewNetSalary = calculator.CalculateNetto(NewGrossSalary, TaxClass, ChurchTax, kirchensteuerRate);
        }





        
        private void CalculateCommuteAndTotalCosts(float parsedFuelPrice, float adjustedFuelConsumption, BruttoNettoCalculator calculator, float kirchensteuerRate)
        {
            float costPerKm = (adjustedFuelConsumption / 100f) * parsedFuelPrice;
            // Berechnung der jährlichen Pendeltage basierend auf den durchschnittlichen Arbeitstagen pro Woche
            int annualWorkDays = AverageCommuteDays * 52; // 52 Wochen im Jahr

            // Berechnung der jährlichen Pendelkosten (Hin- und Rückfahrt, durchschnittlichen Arbeitstage pro woche)
            float annualCommuteCost = CommuteDistance * 2 * costPerKm * annualWorkDays;

            // Monatliche Pendelkosten
            CommuteCost = (float)Math.Round(annualCommuteCost / 12, 2);

            //Jährliche Pendelkosten
            CommuteCostYear = CommuteCost * 12;



            float annualWearAndTearCost = 0;
            if (IncludeWearAndTear)
            {
                WearAndTearCalculator wearAndTearCalculator = new WearAndTearCalculator();
                annualWearAndTearCost = wearAndTearCalculator.CalculateWearAndTear(CommuteDistance * annualWorkDays, WearLevel);
            }

            // Gesamtjahreskosten (Pendelkosten + Abnutzungskosten)
            float totalAnnualCost = annualCommuteCost + annualWearAndTearCost;

            // Monatliche Gesamtkosten
            TotalCost = (float)Math.Round(totalAnnualCost / 12, 2);

            // Monatliche Abnutzungskosten
            MonthlyWearAndTear = TotalCost - CommuteCost;

            // Gesamtjahreskosten (inkl. Pendelkosten und Abnutzung)
            TotalCostWearAndTear = totalAnnualCost;

            //Jährliche Abnutzungskosten
            WearAndTearYear = MonthlyWearAndTear * 12;

            // Anpassung des Jahresgehalts nach Abzug der Gesamtkosten
            float adjustedGrossSalary = NewGrossSalary - totalAnnualCost;
            AdjustedNetSalary = calculator.CalculateNetto(adjustedGrossSalary, TaxClass, ChurchTax, kirchensteuerRate);

            // Berechnung der Gehaltsdifferenz nach Pendelkosten
            //SalaryDifference = (NewGrossSalary - GrossSalary) - totalAnnualCost;

            // Berechnung des angepassten Jahresgehalts
            AdjustedSalary = NewGrossSalary - totalAnnualCost;

            AdjustedNetYearSalary = Math.Max(0,NetSalary - totalAnnualCost);

            IsCalculated = true;
        }
        
        private void CheckSoliThreshold(float grossSalary, int taxClass)
        {
            float soliThreshold = (taxClass == 3 || taxClass == 4) ? 136826f : 68413f;
            IncludeSoli = grossSalary > soliThreshold;
        }

     

    }
}
