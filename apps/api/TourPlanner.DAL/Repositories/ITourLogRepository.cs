using TourPlanner.Models;

namespace TourPlanner.DAL.Repositories;

public interface ITourLogRepository
{
    Task<IEnumerable<TourLog>> GetByTourIdAsync(Guid tourId, string userId, CancellationToken ct = default);
    Task<TourLog?> GetByIdAsync(Guid id, string userId, CancellationToken ct = default);
    Task<bool> ExistsForTourAtDateTimeAsync(Guid tourId, string userId, DateTime dateTime, CancellationToken ct = default);
    Task<TourLog> CreateAsync(TourLog log, CancellationToken ct = default);
    Task<TourLog> UpdateAsync(TourLog log, CancellationToken ct = default);
    Task DeleteAsync(Guid id, string userId, CancellationToken ct = default);
}
