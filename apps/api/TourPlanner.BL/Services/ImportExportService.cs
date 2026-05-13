using Microsoft.Extensions.Logging;
using TourPlanner.BL.Exceptions;
using TourPlanner.DAL.Repositories;
using TourPlanner.Models;

namespace TourPlanner.BL.Services;

public class ImportExportService : IImportExportService
{
    private readonly ITourRepository _tourRepo;
    private readonly ITourLogRepository _logRepo;
    private readonly ILogger<ImportExportService> _logger;

    public ImportExportService(ITourRepository tourRepo, ITourLogRepository logRepo, ILogger<ImportExportService> logger)
    {
        _tourRepo = tourRepo;
        _logRepo = logRepo;
        _logger = logger;
    }

    public async Task<IEnumerable<Tour>> ExportAsync(string userId, CancellationToken ct = default)
        => await _tourRepo.GetAllAsync(userId, ct: ct);

    public async Task<int> ImportAsync(string userId, IEnumerable<Tour> tours, CancellationToken ct = default)
    {
        var list = tours.ToList();
        if (list.Count == 0)
            throw new BusinessLogicException("The import file contains no tours.");

        var imported = 0;
        foreach (var src in list)
        {
            if (string.IsNullOrWhiteSpace(src.Name) || string.IsNullOrWhiteSpace(src.From) || string.IsNullOrWhiteSpace(src.To))
                throw new BusinessLogicException("Each imported tour must have a name, origin and destination.");

            var tour = new Tour
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = await UniqueNameAsync(userId, src.Name.Trim(), ct),
                Description = src.Description,
                From = src.From,
                To = src.To,
                TransportType = src.TransportType,
                Distance = src.Distance,
                EstimatedTimeSeconds = src.EstimatedTimeSeconds,
                RouteInformation = src.RouteInformation,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };
            await _tourRepo.CreateAsync(tour, ct);

            foreach (var log in src.TourLogs)
            {
                await _logRepo.CreateAsync(new TourLog
                {
                    Id = Guid.NewGuid(),
                    TourId = tour.Id,
                    UserId = userId,
                    DateTime = log.DateTime,
                    Comment = log.Comment,
                    Difficulty = log.Difficulty,
                    TotalDistanceKm = log.TotalDistanceKm,
                    TotalTimeSeconds = log.TotalTimeSeconds,
                    Rating = log.Rating,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                }, ct);
            }

            imported++;
        }

        _logger.LogInformation("Imported {Count} tour(s) for user {UserId}", imported, userId);
        return imported;
    }

    private async Task<string> UniqueNameAsync(string userId, string name, CancellationToken ct)
    {
        if (!await _tourRepo.ExistsWithNameAsync(userId, name, ct)) return name;
        for (var n = 2; ; n++)
        {
            var candidate = $"{name} ({n})";
            if (!await _tourRepo.ExistsWithNameAsync(userId, candidate, ct)) return candidate;
        }
    }
}
