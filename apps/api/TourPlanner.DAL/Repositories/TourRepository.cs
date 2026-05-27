using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TourPlanner.DAL.Exceptions;
using TourPlanner.Models;
using TourPlanner.Models.Enums;

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
                query = query.Where(BuildSearchPredicate(searchQuery.Trim()));

            return await query
                .OrderBy(t => t.Name)
                .ToListAsync(ct);
        }
        catch (Exception ex) when (ex is not DataAccessException)
        {
            throw new DataAccessException("Failed to retrieve tours.", ex);
        }
    }

    private static Expression<Func<Tour, bool>> BuildSearchPredicate(string q)
    {
        var like = $"%{q}%";

        var matchingTransports = Enum.GetValues<TransportType>()
            .Where(tt => tt.ToString().Contains(q, StringComparison.OrdinalIgnoreCase))
            .ToArray();

        return t =>
            EF.Functions.ILike(t.Name, like) ||
            EF.Functions.ILike(t.Description, like) ||
            EF.Functions.ILike(t.From, like) ||
            EF.Functions.ILike(t.To, like) ||
            matchingTransports.Contains(t.TransportType) ||
            t.TourLogs.Any(l => EF.Functions.ILike(l.Comment, like)) ||
            // Popularity == TourLogs.Count
            EF.Functions.ILike(t.TourLogs.Count.ToString(), like) ||
            EF.Functions.ILike(
                !t.TourLogs.Any()
                    ? "notsuitable not suitable"
                    : t.TourLogs.Average(l => l.Difficulty == Difficulty.Easy ? 1 : l.Difficulty == Difficulty.Medium ? 2 : 3) <= (int)Difficulty.Easy
                      && t.TourLogs.Average(l => l.TotalDistanceKm) <= 10
                        ? "verysuitable very suitable"
                        : t.TourLogs.Average(l => l.Difficulty == Difficulty.Easy ? 1 : l.Difficulty == Difficulty.Medium ? 2 : 3) <= (int)Difficulty.Medium
                          && t.TourLogs.Average(l => l.TotalDistanceKm) <= 30
                            ? "suitable"
                            : "notsuitable not suitable",
                like);
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
