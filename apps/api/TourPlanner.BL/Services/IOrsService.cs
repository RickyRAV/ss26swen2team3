namespace TourPlanner.BL.Services;

public record RouteResult(
    IReadOnlyList<double[]> Coordinates,
    double DistanceMeters,
    double DurationSeconds
);

public interface IOrsService
{
    Task<RouteResult> GetRouteAsync(string from, string to, string profile, CancellationToken ct = default);
}
