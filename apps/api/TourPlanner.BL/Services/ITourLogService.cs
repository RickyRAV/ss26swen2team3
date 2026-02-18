using TourPlanner.Models;

namespace TourPlanner.BL.Services;

public interface ITourLogService
{
    Task<IEnumerable<TourLog>> GetByTourIdAsync(Guid tourId, string userId, CancellationToken ct = default);
    Task<TourLog> GetByIdAsync(Guid id, string userId, CancellationToken ct = default);
    Task<TourLog> CreateAsync(TourLog log, CancellationToken ct = default);
    Task<TourLog> UpdateAsync(Guid id, string userId, TourLog updates, CancellationToken ct = default);
    Task DeleteAsync(Guid id, string userId, CancellationToken ct = default);
}
