using Moq;
using TourPlanner.DAL.Repositories;
using TourPlanner.Models;

namespace TourPlanner.Tests.Helpers;

public static class MockRepositoryFactory
{
    public static Mock<ITourRepository> TourRepository() => new();
    public static Mock<ITourLogRepository> TourLogRepository() => new();
    public static Mock<IRefreshTokenRepository> RefreshTokenRepository() => new();

    public static Tour CreateTour(string userId = "user-1", string name = "Test Tour") => new()
    {
        Id = Guid.NewGuid(),
        UserId = userId,
        Name = name,
        Description = "A test tour",
        From = "Vienna",
        To = "Graz",
        TransportType = TourPlanner.Models.Enums.TransportType.Car,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    public static TourLog CreateTourLog(Guid? tourId = null, string userId = "user-1") => new()
    {
        Id = Guid.NewGuid(),
        TourId = tourId ?? Guid.NewGuid(),
        UserId = userId,
        DateTime = DateTime.UtcNow.AddDays(-1),
        Comment = "Great tour!",
        Difficulty = TourPlanner.Models.Enums.Difficulty.Medium,
        TotalDistanceKm = 15.5,
        TotalTimeSeconds = 3600,
        Rating = TourPlanner.Models.Enums.Rating.Four,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };
}
