using TourPlanner.Models.Enums;

namespace TourPlanner.Models;

public class Tour
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string UserId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string From { get; set; } = string.Empty;

    public string To { get; set; } = string.Empty;

    public TransportType TransportType { get; set; }

    public double? Distance { get; set; }

    public double? EstimatedTimeSeconds { get; set; }

    // GeoJSON route geometry from ORS
    public string? RouteInformation { get; set; }

    public string? ImagePath { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<TourLog> TourLogs { get; set; } = new List<TourLog>();

    public int Popularity => TourLogs.Count;

    /// <summary>
    /// Child-friendliness derived from recorded difficulty, total times and distance.
    /// VerySuitable: all logs are Easy and distance ≤ 10 km
    /// Suitable:     average difficulty is Easy/Medium and distance ≤ 30 km
    /// NotSuitable:  otherwise
    /// </summary>
    public ChildFriendliness ChildFriendliness
    {
        get
        {
            if (!TourLogs.Any()) return ChildFriendliness.NotSuitable;

            var avgDifficulty = TourLogs.Average(l => (int)l.Difficulty);
            var avgDistance = TourLogs.Average(l => l.TotalDistanceKm);

            if (avgDifficulty <= (int)Difficulty.Easy && avgDistance <= 10)
                return ChildFriendliness.VerySuitable;

            if (avgDifficulty <= (int)Difficulty.Medium && avgDistance <= 30)
                return ChildFriendliness.Suitable;

            return ChildFriendliness.NotSuitable;
        }
    }
}
