using TourPlanner.Models.Enums;

namespace TourPlanner.Api.DTOs;

public record ToursExportFile(
    DateTime ExportedAt,
    int Count,
    IReadOnlyList<TourExportItem> Tours
);

public record TourExportItem(
    string Name,
    string Description,
    string From,
    string To,
    TransportType TransportType,
    double? Distance,
    double? EstimatedTimeSeconds,
    string? RouteInformation,
    IReadOnlyList<TourLogExportItem> Logs
);

public record TourLogExportItem(
    DateTime DateTime,
    string Comment,
    Difficulty Difficulty,
    double TotalDistanceKm,
    double TotalTimeSeconds,
    Rating Rating
);

public record ToursImportFile(
    IReadOnlyList<TourImportItem>? Tours
);

public record TourImportItem(
    string? Name,
    string? Description,
    string? From,
    string? To,
    TransportType TransportType,
    double? Distance,
    double? EstimatedTimeSeconds,
    string? RouteInformation,
    IReadOnlyList<TourLogExportItem>? Logs
);
