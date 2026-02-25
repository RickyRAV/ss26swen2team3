using TourPlanner.Models.Enums;

namespace TourPlanner.Api.DTOs;

public record TourLogDto(
    Guid Id,
    Guid TourId,
    DateTime DateTime,
    string Comment,
    Difficulty Difficulty,
    double TotalDistanceKm,
    double TotalTimeSeconds,
    Rating Rating,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record CreateTourLogRequest(
    DateTime DateTime,
    string Comment,
    Difficulty Difficulty,
    double TotalDistanceKm,
    double TotalTimeSeconds,
    Rating Rating
);

public record UpdateTourLogRequest(
    DateTime DateTime,
    string Comment,
    Difficulty Difficulty,
    double TotalDistanceKm,
    double TotalTimeSeconds,
    Rating Rating
);
