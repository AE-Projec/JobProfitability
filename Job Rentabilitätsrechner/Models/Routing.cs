namespace Job_Rentabilitätsrechner.Models
{
    public class DistanceResult
    {
        public RouteFeature[] features { get; set; } 
    }

    public class RouteFeature
    {
        public Properties properties { get; set; }
    }

    public class Properties
    {
        public Segment[] segments { get; set; }
    }


    public class Segment
    {
        public double? distance { get; set; }
        public double? duration { get; set; }
        public double? durationInSeconds { get; set; }
        public double? fullDistance { get; set; }

        public double? oldDistance { get; set; }
        public double? oldDuration { get; set; }
        public double? oldDurationInSeconds { get; set; }
        public double? oldFullDistance { get; set; }
    }


    public class RouteInfo
    {
        public double? Distance { get; set; } // Distanz in Kilometern
        public int? Duration { get; set; } // Dauer in Minuten
        public int? DurationSeconds { get; set; } 
        public double? FullDistance { get; set; }

        public double? OldDistance { get; set; } // Distanz in Kilometern
        public int? OldDuration { get; set; } // Dauer in Minuten
        public int? OldDurationSeconds { get; set; }
        public double? OldFullDistance { get; set; }


    }


}
