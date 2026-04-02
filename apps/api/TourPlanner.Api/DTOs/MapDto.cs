namespace TourPlanner.Api.DTOs;

public record RouteResponse(
    IReadOnlyList<double[]> Coordinates,
    double DistanceMeters,
    double DurationSeconds
);
