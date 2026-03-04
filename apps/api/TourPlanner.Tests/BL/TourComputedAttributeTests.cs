using NUnit.Framework;
using TourPlanner.Models;
using TourPlanner.Models.Enums;
using TourPlanner.Tests.Helpers;

namespace TourPlanner.Tests.BL;

/// <summary>Tests for computed Tour attributes (Popularity and ChildFriendliness).</summary>
[TestFixture]
public class TourComputedAttributeTests
{
    [Test]
    public void Popularity_NoLogs_ReturnsZero()
    {
        var tour = MockRepositoryFactory.CreateTour();
        Assert.That(tour.Popularity, Is.EqualTo(0));
    }

    [Test]
    public void Popularity_WithLogs_ReturnsLogCount()
    {
        var tour = MockRepositoryFactory.CreateTour();
        tour.TourLogs = new List<TourLog>
        {
            MockRepositoryFactory.CreateTourLog(tour.Id),
            MockRepositoryFactory.CreateTourLog(tour.Id)
        };
        Assert.That(tour.Popularity, Is.EqualTo(2));
    }

    [Test]
    public void ChildFriendliness_NoLogs_ReturnsNotSuitable()
    {
        var tour = MockRepositoryFactory.CreateTour();
        Assert.That(tour.ChildFriendliness, Is.EqualTo(ChildFriendliness.NotSuitable));
    }

    [Test]
    public void ChildFriendliness_AllEasyShortDistance_ReturnsVerySuitable()
    {
        var tour = MockRepositoryFactory.CreateTour();
        tour.TourLogs = new List<TourLog>
        {
            new() { Difficulty = Difficulty.Easy, TotalDistanceKm = 5, TotalTimeSeconds = 1800, Rating = Rating.Five },
            new() { Difficulty = Difficulty.Easy, TotalDistanceKm = 8, TotalTimeSeconds = 2400, Rating = Rating.Four }
        };
        Assert.That(tour.ChildFriendliness, Is.EqualTo(ChildFriendliness.VerySuitable));
    }

    [Test]
    public void ChildFriendliness_MediumDistanceUnder30_ReturnsSuitable()
    {
        var tour = MockRepositoryFactory.CreateTour();
        tour.TourLogs = new List<TourLog>
        {
            new() { Difficulty = Difficulty.Medium, TotalDistanceKm = 20, TotalTimeSeconds = 5400, Rating = Rating.Three }
        };
        Assert.That(tour.ChildFriendliness, Is.EqualTo(ChildFriendliness.Suitable));
    }

    [Test]
    public void ChildFriendliness_HardLongDistance_ReturnsNotSuitable()
    {
        var tour = MockRepositoryFactory.CreateTour();
        tour.TourLogs = new List<TourLog>
        {
            new() { Difficulty = Difficulty.Hard, TotalDistanceKm = 50, TotalTimeSeconds = 14400, Rating = Rating.Two }
        };
        Assert.That(tour.ChildFriendliness, Is.EqualTo(ChildFriendliness.NotSuitable));
    }
}
