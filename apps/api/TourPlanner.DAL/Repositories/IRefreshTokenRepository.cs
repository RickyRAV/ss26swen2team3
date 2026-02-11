using TourPlanner.Models;

namespace TourPlanner.DAL.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default);
    Task CreateAsync(RefreshToken token, CancellationToken ct = default);
    Task RevokeAsync(string token, CancellationToken ct = default);
    Task RevokeAllForUserAsync(string userId, CancellationToken ct = default);
}
