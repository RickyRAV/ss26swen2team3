using Microsoft.Extensions.Logging;
using TourPlanner.BL.Exceptions;
using TourPlanner.DAL.Repositories;
using TourPlanner.Models;

namespace TourPlanner.BL.Services;

public class TourService : ITourService
{
    private readonly ITourRepository _tourRepository;
    private readonly ILogger<TourService> _logger;

    public TourService(ITourRepository tourRepository, ILogger<TourService> logger)
    {
        _tourRepository = tourRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<Tour>> GetAllAsync(string userId, string? searchQuery = null, CancellationToken ct = default)
    {
        _logger.LogInformation("Fetching tours for user {UserId}, search: {Search}", userId, searchQuery ?? "(none)");
        return await _tourRepository.GetAllAsync(userId, searchQuery, ct);
    }

    public async Task<Tour> GetByIdAsync(Guid id, string userId, CancellationToken ct = default)
    {
        var tour = await _tourRepository.GetByIdAsync(id, userId, ct);
        if (tour is null)
        {
            _logger.LogWarning("Tour {TourId} not found for user {UserId}", id, userId);
            throw new TourNotFoundException(id);
        }
        return tour;
    }

    public async Task<Tour> CreateAsync(Tour tour, CancellationToken ct = default)
    {
        var exists = await _tourRepository.ExistsWithNameAsync(tour.UserId, tour.Name, ct);
        if (exists)
            throw new BusinessLogicException($"A tour named \"{tour.Name}\" already exists.");

        _logger.LogInformation("Creating tour '{Name}' for user {UserId}", tour.Name, tour.UserId);
        return await _tourRepository.CreateAsync(tour, ct);
    }

    public async Task<Tour> UpdateAsync(Guid id, string userId, Tour updates, CancellationToken ct = default)
    {
        var existing = await _tourRepository.GetByIdAsync(id, userId, ct);
        if (existing is null) throw new TourNotFoundException(id);

        existing.Name = updates.Name;
        existing.Description = updates.Description;
        existing.From = updates.From;
        existing.To = updates.To;
        existing.TransportType = updates.TransportType;
        existing.Distance = updates.Distance ?? existing.Distance;
        existing.EstimatedTimeSeconds = updates.EstimatedTimeSeconds ?? existing.EstimatedTimeSeconds;
        existing.RouteInformation = updates.RouteInformation ?? existing.RouteInformation;
        existing.ImagePath = updates.ImagePath ?? existing.ImagePath;

        _logger.LogInformation("Updating tour {TourId}", id);
        return await _tourRepository.UpdateAsync(existing, ct);
    }

    public async Task DeleteAsync(Guid id, string userId, CancellationToken ct = default)
    {
        _logger.LogInformation("Deleting tour {TourId} for user {UserId}", id, userId);
        await _tourRepository.DeleteAsync(id, userId, ct);
    }
}
