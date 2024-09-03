namespace Job_Rentabilitätsrechner.Services
{
    public class FuelConsumptionAdjustmentService
    {
        public float AdjustFuelConsumption(float consumption, string? transmissionType,int? gearCount)
        {
            float adjustedConsumption = consumption;

            if (transmissionType == "automatic")
            {
                adjustedConsumption *= 1.10f; // Erhöhung um 10% bei Automatikgetriebe

                if (gearCount.HasValue && gearCount > 5)
                {
                    adjustedConsumption *= 0.95f; // Reduzierung um 5% bei mehr als 5 Gängen
                }
            }

            return adjustedConsumption;
        }
    }
}
