namespace TourPlanner.BL.Services;

public record RouteResult(
    IReadOnlyList<double[]> Coordinates,
    IReadOnlyList<double> Elevations,
    double DistanceMeters,
    double DurationSeconds
);

public interface IOrsService
{
    Task<RouteResult> GetRouteAsync(string from, string to, string profile, CancellationToken ct = default);
}
