using Job_Rentabilitätsrechner.Interfaces;

namespace Job_Rentabilitätsrechner.Services
{
    public class WearAndTearCalculationService : IWearAndTearCalculator
    {
        public float CalculateTotalWearAndTear(float distanceInKm)
        {
            float wearPerKm = 0.18f;  // Standardwert für Abnutzung pro Kilometer
            return distanceInKm * wearPerKm;
        }

        public float CalculateWearAndTear(float distanceInKm, int wearLevel)
        {
            float wearPerKm;

            switch (wearLevel)
            {
                case 1:
                    wearPerKm = 0.12f;
                    break;
                case 2:
                    wearPerKm = 0.18f;
                    break;
                case 3:
                    wearPerKm = 0.25f;
                    break;
                default:
                    wearPerKm = 0.18f;
                    break;
            }

            return distanceInKm * wearPerKm;
        }
    }
}
