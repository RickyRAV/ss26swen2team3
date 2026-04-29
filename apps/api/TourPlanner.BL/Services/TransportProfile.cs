using TourPlanner.Models.Enums;

namespace TourPlanner.BL.Services;

public static class TransportProfile
{
    public static string Resolve(TransportType type) => type switch
    {
        TransportType.Bicycle => "cycling-regular",
        TransportType.Hiking  => "foot-hiking",
        TransportType.Running => "foot-walking",
        _                     => "driving-car", // Car, Vacation
    };
}
