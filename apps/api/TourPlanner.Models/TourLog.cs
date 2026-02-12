using TourPlanner.Models.Enums;

namespace TourPlanner.Models;

public class TourLog
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid TourId { get; set; }

    public string UserId { get; set; } = string.Empty;

    public DateTime DateTime { get; set; } = DateTime.UtcNow;

    public string Comment { get; set; } = string.Empty;

    public Difficulty Difficulty { get; set; }

    /// <summary>Total distance covered during this log entry, in kilometres.</summary>
    public double TotalDistanceKm { get; set; }

    /// <summary>Total time spent during this log entry, in seconds.</summary>
    public double TotalTimeSeconds { get; set; }

    public Rating Rating { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Tour? Tour { get; set; }
}
