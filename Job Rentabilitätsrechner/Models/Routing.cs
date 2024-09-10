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
        public double distance { get; set; }
        public double duration { get; set; }
    }
}
