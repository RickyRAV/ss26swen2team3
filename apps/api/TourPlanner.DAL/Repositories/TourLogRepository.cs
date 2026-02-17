using Microsoft.EntityFrameworkCore;
using TourPlanner.DAL.Exceptions;
using TourPlanner.Models;

namespace TourPlanner.DAL.Repositories;

public class TourLogRepository : ITourLogRepository
{
    private readonly TourPlannerDbContext _context;

    public TourLogRepository(TourPlannerDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TourLog>> GetByTourIdAsync(Guid tourId, string userId, CancellationToken ct = default)
    {
        try
        {
            return await _context.TourLogs
                .Where(l => l.TourId == tourId && l.UserId == userId)
                .OrderByDescending(l => l.DateTime)
                .ToListAsync(ct);
        }
        catch (Exception ex) when (ex is not DataAccessException)
        {
            throw new DataAccessException($"Failed to retrieve logs for tour {tourId}.", ex);
        }
    }

    public async Task<TourLog?> GetByIdAsync(Guid id, string userId, CancellationToken ct = default)
    {
        try
        {
            return await _context.TourLogs
                .FirstOrDefaultAsync(l => l.Id == id && l.UserId == userId, ct);
        }
        catch (Exception ex) when (ex is not DataAccessException)
        {
            throw new DataAccessException($"Failed to retrieve tour log {id}.", ex);
        }
    }

    public async Task<bool> ExistsForTourAtDateTimeAsync(Guid tourId, string userId, DateTime dateTime, CancellationToken ct = default)
    {
        try
        {
            return await _context.TourLogs
                .AnyAsync(l => l.TourId == tourId && l.UserId == userId && l.DateTime == dateTime, ct);
        }
        catch (Exception ex) when (ex is not DataAccessException)
        {
            throw new DataAccessException("Failed to check tour log uniqueness.", ex);
        }
    }

    public async Task<TourLog> CreateAsync(TourLog log, CancellationToken ct = default)
    {
        try
        {
            _context.TourLogs.Add(log);
            await _context.SaveChangesAsync(ct);
            return log;
        }
        catch (Exception ex) when (ex is not DataAccessException)
        {
            throw new DataAccessException("Failed to create tour log.", ex);
        }
    }

    public async Task<TourLog> UpdateAsync(TourLog log, CancellationToken ct = default)
    {
        try
        {
            log.UpdatedAt = DateTime.UtcNow;
            _context.TourLogs.Update(log);
            await _context.SaveChangesAsync(ct);
            return log;
        }
        catch (Exception ex) when (ex is not DataAccessException)
        {
            throw new DataAccessException($"Failed to update tour log {log.Id}.", ex);
        }
    }

    public async Task DeleteAsync(Guid id, string userId, CancellationToken ct = default)
    {
        try
        {
            var log = await _context.TourLogs
                .FirstOrDefaultAsync(l => l.Id == id && l.UserId == userId, ct);

            if (log is null) return;

            _context.TourLogs.Remove(log);
            await _context.SaveChangesAsync(ct);
        }
        catch (Exception ex) when (ex is not DataAccessException)
        {
            throw new DataAccessException($"Failed to delete tour log {id}.", ex);
        }
    }
}
