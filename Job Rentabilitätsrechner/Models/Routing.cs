namespace Job_Rentabilitätsrechner.Models
{
    public class DistanceResult
    {
        public RouteFeature[] features { get; set; }  // Umbenannt zu RouteFeature
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
        public double durationInSeconds { get; set; }
    }

    
    public class RouteInfo
    {
        public double? Distance { get; set; } // Distanz in Kilometern
        public int? Duration { get; set; } // Dauer in Minuten
        public int? DurationSeconds { get; set; } 
    }


}
