using Job_Rentabilitätsrechner.Pages;
using System.Formats.Asn1;

namespace Job_Rentabilitätsrechner.Services
{
    public class CommuteCostService
    {
        private readonly NetSalaryCalculationService _netSalaryCalculationService;
        public CommuteCostService(NetSalaryCalculationService netSalaryCalculationService)
        {
            _netSalaryCalculationService = netSalaryCalculationService;
        }

        public void CalculateCommuteAndTotalCosts(float parsedFuelPrice,
            float adjustedFuelConsumption,
            float commuteDistance,
            int averageCommuteDays,
            int wearLevel,
            bool includeWearAndTear,
            float grossSalary,
            float newGrossSalary,
            float taxClass,
            bool churchTax,
            float kirchensteuerRate,
            out float commuteCost,
            out float commuteCostYear,
            out float totalCost,
            out float monthlyWearAndTear,
            out float totalCostWearAndTear,
            out float wearAndTearYear,
            out float adjustedNetSalary,
            out float salaryDifference,
            out float adjustedSalary)
        {
            float costPerKm = (adjustedFuelConsumption / 100f) * parsedFuelPrice;
            // Berechnung der jährlichen Pendeltage basierend auf den durchschnittlichen Arbeitstagen pro Woche
            int annualWorkDays = averageCommuteDays * 52; // 52 Wochen im Jahr

            // Berechnung der jährlichen Pendelkosten (Hin- und Rückfahrt, durchschnittlichen Arbeitstage pro woche)
            float annualCommuteCost = commuteDistance * 2 * costPerKm * annualWorkDays;

            // Monatliche Pendelkosten
            commuteCost = (float)Math.Round(annualCommuteCost / 12, 2);

            //Jährliche Pendelkosten
            commuteCostYear = commuteCost * 12;



            float annualWearAndTearCost = 0;
            if (includeWearAndTear)
            {
                WearAndTearCalculator wearAndTearCalculator = new WearAndTearCalculator();
                annualWearAndTearCost = wearAndTearCalculator.CalculateWearAndTear(commuteDistance * annualWorkDays, wearLevel);
            }

            // Gesamtjahreskosten (Pendelkosten + Abnutzungskosten)
            float totalAnnualCost = annualCommuteCost + annualWearAndTearCost;

            // Monatliche Gesamtkosten
            totalCost = (float)Math.Round(totalAnnualCost / 12, 2);

            // Monatliche Abnutzungskosten
            monthlyWearAndTear = totalCost - commuteCost;

            // Gesamtjahreskosten (inkl. Pendelkosten und Abnutzung)
            totalCostWearAndTear = totalAnnualCost;

            //Jährliche Abnutzungskosten
            wearAndTearYear = monthlyWearAndTear * 12;

            // Anpassung des Jahresgehalts nach Abzug der Gesamtkosten
            float adjustedGrossSalary = newGrossSalary - totalAnnualCost;
            adjustedNetSalary = _netSalaryCalculationService.CalculateNetSalary(adjustedGrossSalary, (int)taxClass, churchTax, kirchensteuerRate);

            // Berechnung der Gehaltsdifferenz nach Pendelkosten
            salaryDifference = (newGrossSalary - grossSalary) - totalAnnualCost;

            // Berechnung des angepassten Jahresgehalts
            adjustedSalary = newGrossSalary - totalAnnualCost;


        }
    }
}
