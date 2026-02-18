using TourPlanner.Models;

namespace TourPlanner.BL.Services;

public interface ITourService
{
    Task<IEnumerable<Tour>> GetAllAsync(string userId, string? searchQuery = null, CancellationToken ct = default);
    Task<Tour> GetByIdAsync(Guid id, string userId, CancellationToken ct = default);
    Task<Tour> CreateAsync(Tour tour, CancellationToken ct = default);
    Task<Tour> UpdateAsync(Guid id, string userId, Tour updates, CancellationToken ct = default);
    Task DeleteAsync(Guid id, string userId, CancellationToken ct = default);
}
