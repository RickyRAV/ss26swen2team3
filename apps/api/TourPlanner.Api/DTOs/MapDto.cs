namespace TourPlanner.Api.DTOs;

public record RouteResponse(
    IReadOnlyList<double[]> Coordinates,
    IReadOnlyList<double> Elevations,
    double DistanceMeters,
    double DurationSeconds
);
