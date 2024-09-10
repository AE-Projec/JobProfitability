using Job_Rentabilitätsrechner.Pages;

namespace Job_Rentabilitätsrechner.Interfaces
{
    public interface ICommuteCostCalculationService
    {
        float CalculateCommuteCosts(float fuelPrice, float fuelConsumption, float commuteDistance, int averageCommuteDays);
        float CalculateWearAndTearCosts(float commuteDistance, int averageCommuteDays, int wearLevel, bool includeWearAndTear);

        void CalculateCommuteAndTotalCosts(
           float parsedFuelPrice,
            float adjustedFuelConsumption,
            float commuteDistance,
            int averageCommuteDays,
            int wearLevel,
            bool includeWearAndTear,
            float grossSalary,
            float newGrossSalary,
            int taxClass,
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
            out float adjustedSalary,
            out float totalAnnualCost);
    }
}
