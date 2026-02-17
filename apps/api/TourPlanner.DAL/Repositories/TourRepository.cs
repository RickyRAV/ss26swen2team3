using Microsoft.EntityFrameworkCore;
using TourPlanner.DAL.Exceptions;
using TourPlanner.Models;

namespace TourPlanner.DAL.Repositories;

public class TourRepository : ITourRepository
{
    private readonly TourPlannerDbContext _context;

    public TourRepository(TourPlannerDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Tour>> GetAllAsync(string userId, string? searchQuery = null, CancellationToken ct = default)
    {
        try
        {
            var query = _context.Tours
                .Include(t => t.TourLogs)
                .Where(t => t.UserId == userId);

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var q = searchQuery.ToLower();
                query = query.Where(t =>
                    t.Name.ToLower().Contains(q) ||
                    t.Description.ToLower().Contains(q) ||
                    t.From.ToLower().Contains(q) ||
                    t.To.ToLower().Contains(q) ||
                    t.TourLogs.Any(l => l.Comment.ToLower().Contains(q)));
            }

            return await query.OrderBy(t => t.Name).ToListAsync(ct);
        }
        catch (Exception ex) when (ex is not DataAccessException)
        {
            throw new DataAccessException("Failed to retrieve tours.", ex);
        }
    }

    public async Task<Tour?> GetByIdAsync(Guid id, string userId, CancellationToken ct = default)
    {
        try
        {
            return await _context.Tours
                .Include(t => t.TourLogs.OrderByDescending(l => l.DateTime))
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId, ct);
        }
        catch (Exception ex) when (ex is not DataAccessException)
        {
            throw new DataAccessException($"Failed to retrieve tour {id}.", ex);
        }
    }

    public async Task<bool> ExistsWithNameAsync(string userId, string name, CancellationToken ct = default)
    {
        try
        {
            return await _context.Tours
                .AnyAsync(t => t.UserId == userId && t.Name.ToLower() == name.ToLower(), ct);
        }
        catch (Exception ex) when (ex is not DataAccessException)
        {
            throw new DataAccessException("Failed to check tour name uniqueness.", ex);
        }
    }

    public async Task<Tour> CreateAsync(Tour tour, CancellationToken ct = default)
    {
        try
        {
            _context.Tours.Add(tour);
            await _context.SaveChangesAsync(ct);
            return tour;
        }
        catch (Exception ex) when (ex is not DataAccessException)
        {
            throw new DataAccessException("Failed to create tour.", ex);
        }
    }

    public async Task<Tour> UpdateAsync(Tour tour, CancellationToken ct = default)
    {
        try
        {
            tour.UpdatedAt = DateTime.UtcNow;
            _context.Tours.Update(tour);
            await _context.SaveChangesAsync(ct);
            return tour;
        }
        catch (Exception ex) when (ex is not DataAccessException)
        {
            throw new DataAccessException($"Failed to update tour {tour.Id}.", ex);
        }
    }

    public async Task DeleteAsync(Guid id, string userId, CancellationToken ct = default)
    {
        try
        {
            var tour = await _context.Tours
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId, ct);

            if (tour is null) return;

            _context.Tours.Remove(tour);
            await _context.SaveChangesAsync(ct);
        }
        catch (Exception ex) when (ex is not DataAccessException)
        {
            throw new DataAccessException($"Failed to delete tour {id}.", ex);
        }
    }
}
