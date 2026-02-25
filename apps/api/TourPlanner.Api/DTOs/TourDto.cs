using TourPlanner.Models.Enums;

namespace TourPlanner.Api.DTOs;

public record TourDto(
    Guid Id,
    string Name,
    string Description,
    string From,
    string To,
    TransportType TransportType,
    double? Distance,
    double? EstimatedTimeSeconds,
    string? RouteInformation,
    string? ImagePath,
    int Popularity,
    string ChildFriendliness,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record CreateTourRequest(
    string Name,
    string Description,
    string From,
    string To,
    TransportType TransportType
);

public record UpdateTourRequest(
    string Name,
    string Description,
    string From,
    string To,
    TransportType TransportType,
    double? Distance,
    double? EstimatedTimeSeconds,
    string? RouteInformation
);
