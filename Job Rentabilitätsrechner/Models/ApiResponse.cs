namespace Job_Rentabilitätsrechner.Models
{
    public class ApiResponse
    {
        public float? Distance { get; set; }
        public int? Duration { get; set; }
        public int? DurationInSeconds { get; set; }
        public float? FullDistance { get; set; }

        public float? OldDistance { get; set; }
        public int? OldDuration { get; set; }
        public int? OldDurationInSeconds { get; set; }
        public float? OldFullDistance { get; set; }
    }
}
