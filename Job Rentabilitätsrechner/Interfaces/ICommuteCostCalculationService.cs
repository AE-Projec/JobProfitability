using Job_Rentabilitätsrechner.Pages;

namespace Job_Rentabilitätsrechner.Interfaces
{
    public interface ICommuteCostCalculationService
    {
        float CalculateCommuteCosts(float fuelPrice, float fuelConsumption, float commuteDistance, int averageCommuteDays);
        float CalculateWearAndTearCosts(float commuteDistance, int averageCommuteDays, int wearLevel, bool includeWearAndTear);

        //neuer job
        void CalculateCommuteAndTotalCosts(
           float parsedFuelPrice,
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
            );
        
        //alter job
        void OldCalculateCommuteAndTotalCosts(
           float oldParsedFuelPrice,
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
            );
    }
}
