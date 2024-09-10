namespace Job_Rentabilitätsrechner.Interfaces
{
    public interface IFuelConsumptionAdjustment
    {
        float AdjustFuelConsumption(float consumption, string? transmissionType, int? gearCount);
    }
}
