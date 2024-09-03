using Job_Rentabilitätsrechner.Pages;

namespace Job_Rentabilitätsrechner.Services
{
    public class CommuteCostCalculationService
    {

        public float BerechnePendelkosten(float fuelPrice, float fuelConsumption, float commuteDistance, int averageCommuteDays)
        {
            float costPerKm = fuelConsumption / 100f * fuelPrice;
            int annualWorkDays = averageCommuteDays * 52;
            return commuteDistance * 2 * costPerKm * annualWorkDays;
        }

        public float BerechneAbnutzungskosten(float commuteDistance, int averageCommuteDays, int wearLevel, bool includeWearAndTear)
        {
            if (!includeWearAndTear) return 0;

            WearAndTearCalculator wearAndTearCalculator = new WearAndTearCalculator();
            int annualWorkDays = averageCommuteDays * 52;
            return wearAndTearCalculator.CalculateWearAndTear(commuteDistance * annualWorkDays, wearLevel);
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
            NetSalaryCalculationService netSalaryCalculationService,
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
            adjustedNetSalary = netSalaryCalculationService.CalculateNetSalary(adjustedGrossSalary, (int)taxClass, churchTax, kirchensteuerRate);

            // Berechnung der Gehaltsdifferenz nach Pendelkosten
            salaryDifference = (newGrossSalary - grossSalary) - totalAnnualCost;

            // Berechnung des angepassten Jahresgehalts
            adjustedSalary = newGrossSalary - totalAnnualCost;


        }
    }
}
