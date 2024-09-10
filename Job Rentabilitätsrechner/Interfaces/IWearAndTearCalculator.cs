namespace Job_Rentabilitätsrechner.Interfaces
{
    public interface IWearAndTearCalculator
    {
        float CalculateWearAndTear(float distanceInKm, int wearLevel);
        float CalculateTotalWearAndTear(float distanceInKm);
    }
}
