namespace Job_Rentabilitätsrechner.Interfaces
{
    public interface IGeocodingService
    {
        Task<(double Longitude, double Latitude)?> GetCoordinatesAsync(string address);
    }
}
