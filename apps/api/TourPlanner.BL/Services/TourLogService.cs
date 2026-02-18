using Microsoft.Extensions.Logging;
using TourPlanner.BL.Exceptions;
using TourPlanner.DAL.Repositories;
using TourPlanner.Models;

namespace TourPlanner.BL.Services;

public class TourLogService : ITourLogService
{
    private readonly ITourLogRepository _logRepository;
    private readonly ITourRepository _tourRepository;
    private readonly ILogger<TourLogService> _logger;

    public TourLogService(
        ITourLogRepository logRepository,
        ITourRepository tourRepository,
        ILogger<TourLogService> logger)
    {
        _logRepository = logRepository;
        _tourRepository = tourRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<TourLog>> GetByTourIdAsync(Guid tourId, string userId, CancellationToken ct = default)
    {
        var tour = await _tourRepository.GetByIdAsync(tourId, userId, ct);
        if (tour is null) throw new TourNotFoundException(tourId);

        return await _logRepository.GetByTourIdAsync(tourId, userId, ct);
    }

    public async Task<TourLog> GetByIdAsync(Guid id, string userId, CancellationToken ct = default)
    {
        var log = await _logRepository.GetByIdAsync(id, userId, ct);
        if (log is null) throw new TourLogNotFoundException(id);
        return log;
    }

    public async Task<TourLog> CreateAsync(TourLog log, CancellationToken ct = default)
    {
        var tour = await _tourRepository.GetByIdAsync(log.TourId, log.UserId, ct);
        if (tour is null) throw new TourNotFoundException(log.TourId);

        var exists = await _logRepository.ExistsForTourAtDateTimeAsync(log.TourId, log.UserId, log.DateTime, ct);
        if (exists)
            throw new BusinessLogicException($"A log entry for this tour at {log.DateTime:yyyy-MM-dd HH:mm} already exists.");

        _logger.LogInformation("Creating tour log for tour {TourId}", log.TourId);
        return await _logRepository.CreateAsync(log, ct);
    }

    public async Task<TourLog> UpdateAsync(Guid id, string userId, TourLog updates, CancellationToken ct = default)
    {
        var existing = await _logRepository.GetByIdAsync(id, userId, ct);
        if (existing is null) throw new TourLogNotFoundException(id);

        existing.DateTime = updates.DateTime;
        existing.Comment = updates.Comment;
        existing.Difficulty = updates.Difficulty;
        existing.TotalDistanceKm = updates.TotalDistanceKm;
        existing.TotalTimeSeconds = updates.TotalTimeSeconds;
        existing.Rating = updates.Rating;

        _logger.LogInformation("Updating tour log {LogId}", id);
        return await _logRepository.UpdateAsync(existing, ct);
    }

    public async Task DeleteAsync(Guid id, string userId, CancellationToken ct = default)
    {
        _logger.LogInformation("Deleting tour log {LogId} for user {UserId}", id, userId);
        await _logRepository.DeleteAsync(id, userId, ct);
    }
}
