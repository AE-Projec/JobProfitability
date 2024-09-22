using Job_Rentabilitätsrechner.Models;
namespace Job_Rentabilitätsrechner.Interfaces
{
    public interface IDistanceService
    {
        Task<double?> GetDistanceAsync((double Longitude, double Latitude) fromCoords, (double Longitude, double Latitude) toCoords);
        Task<RouteInfo?> GetDurationAsync((double Longitude, double Latitude) fromCoords, (double Longitude, double Latitude) toCoords);
    }

}
