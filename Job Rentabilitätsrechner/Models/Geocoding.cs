namespace Job_Rentabilitätsrechner.Models
{
    public class GeocodeResult
    {
        public GeocodeFeature[] features { get; set; } 
    }

    public class GeocodeFeature
    {
        public Geometry geometry { get; set; }
    }

    public class Geometry
    {
        public double[] coordinates { get; set; }
    }

}
