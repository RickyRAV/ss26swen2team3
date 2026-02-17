using TourPlanner.Models;

namespace TourPlanner.DAL.Repositories;

public interface ITourRepository
{
    Task<IEnumerable<Tour>> GetAllAsync(string userId, string? searchQuery = null, CancellationToken ct = default);
    Task<Tour?> GetByIdAsync(Guid id, string userId, CancellationToken ct = default);
    Task<bool> ExistsWithNameAsync(string userId, string name, CancellationToken ct = default);
    Task<Tour> CreateAsync(Tour tour, CancellationToken ct = default);
    Task<Tour> UpdateAsync(Tour tour, CancellationToken ct = default);
    Task DeleteAsync(Guid id, string userId, CancellationToken ct = default);
}
