using Job_Rentabilitätsrechner.Controller;
using Job_Rentabilitätsrechner.Interfaces;
using Job_Rentabilitätsrechner.Pages;
using System.Formats.Asn1;

namespace Job_Rentabilitätsrechner.Services
{
    public class CommuteCostCalculationService : ICommuteCostCalculationService
    {
        private readonly IWearAndTearCalculator _wearAndTearCalculator;
        private readonly INetSalaryCalculationService _netSalaryCalculationService;

        public CommuteCostCalculationService(IWearAndTearCalculator wearAndTearCalculator, 
            INetSalaryCalculationService netSalaryCalculationService)
        {
            _wearAndTearCalculator = wearAndTearCalculator;
            _netSalaryCalculationService = netSalaryCalculationService;
        }

        public float CalculateCommuteCosts(float fuelPrice, float fuelConsumption, float commuteDistance, int averageCommuteDays)
        {
            float costPerKm = fuelConsumption / 100f * fuelPrice;
            int annualWorkDays = averageCommuteDays * 52;
            return commuteDistance * 2 * costPerKm * annualWorkDays;
        }

        public float CalculateWearAndTearCosts(float commuteDistance, int averageCommuteDays, int wearLevel, bool includeWearAndTear)
        {
            if (!includeWearAndTear) return 0;

            int annualWorkDays = averageCommuteDays * 52;
            return _wearAndTearCalculator.CalculateWearAndTear(commuteDistance * annualWorkDays, wearLevel);
        }

        public void CalculateCommuteAndTotalCosts(float parsedFuelPrice,
            float adjustedFuelConsumption,
            float commuteDistance,
            int averageCommuteDays,
            int wearLevel,
            bool includeWearAndTear,
            float newGrossSalary,
            int taxClass,
            bool churchTax,
            bool isSachsen,
            float kirchensteuerRate,
            out float commuteCost,
            out float commuteCostYear,
            out float totalCost,
            out float monthlyWearAndTear,
            out float totalCostWearAndTear,
            out float wearAndTearYear,
            out float adjustedNetSalary,
            out float adjustedSalary,
            out float totalAnnualCost
           )
        {

            float costPerKm = (adjustedFuelConsumption / 100f) * parsedFuelPrice;
            // Berechnung der jährlichen Pendeltage basierend auf den durchschnittlichen Arbeitstagen pro Woche
            int annualWorkDays = averageCommuteDays * 52; // 52 Wochen im Jahr

            // Berechnung der jährlichen Pendelkosten (Hin- und Rückfahrt, durchschnittlichen Arbeitstage pro woche)
            float annualCommuteCost = commuteDistance  * costPerKm * annualWorkDays; // vorher commuteDistance * 2 * costPerKm

            // Monatliche Pendelkosten
            commuteCost = (float)Math.Round(annualCommuteCost / 12, 2);

            //Jährliche Pendelkosten
            commuteCostYear = annualCommuteCost; // vorher commuteCost * 12

            float annualWearAndTearCost = 0;
            if (includeWearAndTear)
            {
                annualWearAndTearCost = _wearAndTearCalculator.CalculateWearAndTear(commuteDistance * annualWorkDays, wearLevel);
            }

            // Gesamtjahreskosten (Pendelkosten + Abnutzungskosten)
            totalAnnualCost = annualCommuteCost + annualWearAndTearCost;

            // Monatliche Gesamtkosten
            totalCost = (float)Math.Round(totalAnnualCost / 12, 2);

            // Monatliche Abnutzungskosten
            //monthlyWearAndTear = totalCost - commuteCost;
            monthlyWearAndTear = annualWearAndTearCost / 12;

            // Gesamtjahreskosten (inkl. Pendelkosten und Abnutzung)
            totalCostWearAndTear = totalAnnualCost;

            //Jährliche Abnutzungskosten
            wearAndTearYear = monthlyWearAndTear * 12;

            // Anpassung des Jahresgehalts nach Abzug der Gesamtkosten Brutto
            float adjustedGrossSalary = newGrossSalary - totalAnnualCost;
            // Angepassted Jahres Nettogehalt
            adjustedNetSalary = _netSalaryCalculationService.CalculateNetSalary(adjustedGrossSalary, taxClass, churchTax, kirchensteuerRate, isSachsen);

            // Berechnung der Gehaltsdifferenz nach Pendelkosten
            //salaryDifference = (newGrossSalary - grossSalary) - totalAnnualCost;

            // Berechnung des angepassten Jahresgehalts
            adjustedSalary = newGrossSalary - totalAnnualCost;
        }



        public void OldCalculateCommuteAndTotalCosts(float oldParsedFuelPrice,
                float oldAdjustedFuelConsumption,
                float oldCommuteDistance,
                int oldAverageCommuteDays,
                int oldWearLevel,
                bool oldIncludeWearAndTear,
                float oldGrossSalary,
                int oldTaxClass,
                bool oldChurchTax,
                bool oldIsSachsen,
                float oldKirchensteuerRate,
                out float oldCommuteCost,
                out float oldCommuteCostYear,
                out float oldTotalCost,
                out float oldMonthlyWearAndTear,
                out float oldTotalCostWearAndTear,
                out float oldWearAndTearYear,
                out float oldAdjustedNetSalary,                
                out float oldAdjustedSalary,
                out float oldTotalAnnualCost
               )
        {
            //System.Diagnostics.Debug.WriteLine($"Old Commute Distance CommuteCostCalculationService: {oldCommuteDistance}, Days: {oldAverageCommuteDays}");
            float oldCostPerKm = (oldAdjustedFuelConsumption / 100f) * oldParsedFuelPrice;
            // Berechnung der jährlichen Pendeltage basierend auf den durchschnittlichen Arbeitstagen pro Woche
            int oldAnnualWorkDays = oldAverageCommuteDays * 52; // 52 Wochen im Jahr

            // Berechnung der jährlichen Pendelkosten (Hin- und Rückfahrt, durchschnittlichen Arbeitstage pro woche)
            float oldAnnualCommuteCost = oldCommuteDistance * oldCostPerKm * oldAnnualWorkDays; // vorher commuteDistance * 2 * costPerKm
            

            //System.Diagnostics.Debug.WriteLine($"Old Annual Commute Cost CommuteCostCalculationService: {oldAnnualCommuteCost}");

            // Monatliche Pendelkosten
            oldCommuteCost = (float)Math.Round(oldAnnualCommuteCost / 12, 2);

            //Jährliche Pendelkosten
            oldCommuteCostYear = oldAnnualCommuteCost; // vorher commuteCost * 12

            float oldAnnualWearAndTearCost = 0;
            if (oldIncludeWearAndTear)
            {
                oldAnnualWearAndTearCost = _wearAndTearCalculator.CalculateWearAndTear(oldCommuteDistance * oldAnnualWorkDays, oldWearLevel);
                //System.Diagnostics.Debug.WriteLine($"Old Annual Wear And Tear Cost CommuteCostCalculationService: {oldAnnualWearAndTearCost}");
            }

            // Gesamtjahreskosten (Pendelkosten + Abnutzungskosten)
            oldTotalAnnualCost = oldAnnualCommuteCost + oldAnnualWearAndTearCost;

            // Monatliche Gesamtkosten
            oldTotalCost = (float)Math.Round(oldTotalAnnualCost / 12, 2);

            // Monatliche Abnutzungskosten
            //monthlyWearAndTear = totalCost - commuteCost;
            oldMonthlyWearAndTear = oldAnnualWearAndTearCost / 12;

            // Gesamtjahreskosten (inkl. Pendelkosten und Abnutzung)
            oldTotalCostWearAndTear = oldTotalAnnualCost;

            //Jährliche Abnutzungskosten
            oldWearAndTearYear = oldMonthlyWearAndTear * 12;

            // Anpassung des Jahresgehalts nach Abzug der Gesamtkosten Brutto
            float oldAdjustedGrossSalary = oldGrossSalary - oldTotalAnnualCost;
            // Angepasstes Jahres Nettogehalt
            oldAdjustedNetSalary = _netSalaryCalculationService.CalculateNetSalary(oldAdjustedGrossSalary, oldTaxClass, oldChurchTax, oldKirchensteuerRate, oldIsSachsen);
            // Berechnung der Gehaltsdifferenz nach Pendelkosten

            // Berechnung des angepassten Jahresgehalts  
            oldAdjustedSalary = oldGrossSalary - oldTotalAnnualCost;
        }


    }
}
