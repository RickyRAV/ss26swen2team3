using System.Text.Json;
using Microsoft.Extensions.Logging;
using TourPlanner.Models;

namespace TourPlanner.BL.Services;

public static class RouteEnricher
{
    private static readonly JsonSerializerOptions Json = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static async Task<bool> EnrichAsync(Tour tour, IOrsService ors, ILogger logger, CancellationToken ct = default)
    {
        try
        {
            var profile = TransportProfile.Resolve(tour.TransportType);
            var route = await ors.GetRouteAsync(tour.From, tour.To, profile, ct);

            tour.Distance = route.DistanceMeters / 1000.0; // stored in km (UI formats km)
            tour.EstimatedTimeSeconds = route.DurationSeconds;
            tour.RouteInformation = JsonSerializer.Serialize(new
            {
                coordinates = route.Coordinates,
                elevations = route.Elevations,
                distanceMeters = route.DistanceMeters,
                durationSeconds = route.DurationSeconds,
            }, Json);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex,
                "Could not compute ORS route for tour '{Name}' ({From} -> {To}); saving without route data.",
                tour.Name, tour.From, tour.To);
            return false;
        }
    }
}
