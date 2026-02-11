using Microsoft.EntityFrameworkCore;
using TourPlanner.DAL.Exceptions;
using TourPlanner.Models;

namespace TourPlanner.DAL.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly TourPlannerDbContext _context;

    public RefreshTokenRepository(TourPlannerDbContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default)
    {
        try
        {
            return await _context.RefreshTokens
                .FirstOrDefaultAsync(r => r.Token == token && !r.IsRevoked && r.ExpiresAt > DateTime.UtcNow, ct);
        }
        catch (Exception ex) when (ex is not DataAccessException)
        {
            throw new DataAccessException("Failed to retrieve refresh token.", ex);
        }
    }

    public async Task CreateAsync(RefreshToken token, CancellationToken ct = default)
    {
        try
        {
            _context.RefreshTokens.Add(token);
            await _context.SaveChangesAsync(ct);
        }
        catch (Exception ex) when (ex is not DataAccessException)
        {
            throw new DataAccessException("Failed to store refresh token.", ex);
        }
    }

    public async Task RevokeAsync(string token, CancellationToken ct = default)
    {
        try
        {
            var entity = await _context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == token, ct);
            if (entity is null) return;
            entity.IsRevoked = true;
            await _context.SaveChangesAsync(ct);
        }
        catch (Exception ex) when (ex is not DataAccessException)
        {
            throw new DataAccessException("Failed to revoke refresh token.", ex);
        }
    }

    public async Task RevokeAllForUserAsync(string userId, CancellationToken ct = default)
    {
        try
        {
            var tokens = await _context.RefreshTokens
                .Where(r => r.UserId == userId && !r.IsRevoked)
                .ToListAsync(ct);
            foreach (var t in tokens) t.IsRevoked = true;
            await _context.SaveChangesAsync(ct);
        }
        catch (Exception ex) when (ex is not DataAccessException)
        {
            throw new DataAccessException("Failed to revoke all refresh tokens for user.", ex);
        }
    }
}
