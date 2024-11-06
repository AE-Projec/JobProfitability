namespace Job_Rentabilitätsrechner.Helpers
{
    public class CommuteCalculationHelper
    {
        public static (float CommuteDistance, float CommuteDuration, string FormattedDuration) CalculateForOldMobileNewNonMobile(float commuteDistanceManual)
        {
            var commuteDuration = (commuteDistanceManual / 50) * 60;
            commuteDuration = (float)Math.Round(commuteDuration, 2);
            var formattedDuration = ConvertMinutesToTime(commuteDuration);

            return (commuteDistanceManual, commuteDuration, formattedDuration);
        }

        public static (float OldCommuteDistance, float OldCommuteDuration, string OldFormattedDuration) CalculateForNewMobileOldNonMobile(float oldCommuteDistanceManual)
        {
            var oldCommuteDuration = (oldCommuteDistanceManual / 50) * 60;
            oldCommuteDuration = (float)Math.Round(oldCommuteDuration, 2);
            var oldFormattedDuration = ConvertMinutesToTime(oldCommuteDuration);

            return (oldCommuteDistanceManual, oldCommuteDuration, oldFormattedDuration);
        }

        public static (float CommuteDistance, float OldCommuteDistance, float CommuteDuration, float OldCommuteDuration, string FormattedDuration, string OldFormattedDuration) CalculateForBothNonMobile(float commuteDistanceManual, float oldCommuteDistanceManual)
        {
            var commuteDuration = (commuteDistanceManual / 50) * 60;
            commuteDuration = (float)Math.Round(commuteDuration, 2);
            var formattedDuration = ConvertMinutesToTime(commuteDuration);

            var oldCommuteDuration = (oldCommuteDistanceManual / 50) * 60;
            oldCommuteDuration = (float)Math.Round(oldCommuteDuration, 2);
            var oldFormattedDuration = ConvertMinutesToTime(oldCommuteDuration);

            return (commuteDistanceManual, oldCommuteDistanceManual, commuteDuration, oldCommuteDuration, formattedDuration, oldFormattedDuration);
        }

        public static (float CommuteDistance, float CommuteDuration, float AverageCommuteDays, string FormattedDuration) SetNoCommuteForBoth()
        {
            return (0, 0, 0, "Keine Pendelzeit");
        }

        private static string ConvertMinutesToTime(float minutes)
        {
            int hours = (int)minutes / 60;
            int mins = (int)minutes % 60;
            return $"{hours}h {mins}m";
        }
    }
}

