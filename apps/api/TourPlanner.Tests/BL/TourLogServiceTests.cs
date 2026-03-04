using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using TourPlanner.BL.Exceptions;
using TourPlanner.BL.Services;
using TourPlanner.DAL.Repositories;
using TourPlanner.Models;
using TourPlanner.Tests.Helpers;

namespace TourPlanner.Tests.BL;

[TestFixture]
public class TourLogServiceTests
{
    private Mock<ITourLogRepository> _logRepoMock = null!;
    private Mock<ITourRepository> _tourRepoMock = null!;
    private Mock<ILogger<TourLogService>> _loggerMock = null!;
    private TourLogService _service = null!;
    private const string UserId = "user-1";

    [SetUp]
    public void SetUp()
    {
        _logRepoMock = MockRepositoryFactory.TourLogRepository();
        _tourRepoMock = MockRepositoryFactory.TourRepository();
        _loggerMock = new Mock<ILogger<TourLogService>>();
        _service = new TourLogService(_logRepoMock.Object, _tourRepoMock.Object, _loggerMock.Object);
    }

    [Test]
    public async Task GetByTourIdAsync_ExistingTour_ReturnsLogs()
    {
        var tour = MockRepositoryFactory.CreateTour(UserId);
        var logs = new List<TourLog> { MockRepositoryFactory.CreateTourLog(tour.Id, UserId) };
        _tourRepoMock.Setup(r => r.GetByIdAsync(tour.Id, UserId, default)).ReturnsAsync(tour);
        _logRepoMock.Setup(r => r.GetByTourIdAsync(tour.Id, UserId, default)).ReturnsAsync(logs);

        var result = await _service.GetByTourIdAsync(tour.Id, UserId);

        Assert.That(result.Count(), Is.EqualTo(1));
    }

    [Test]
    public void GetByTourIdAsync_TourNotFound_ThrowsTourNotFoundException()
    {
        var tourId = Guid.NewGuid();
        _tourRepoMock.Setup(r => r.GetByIdAsync(tourId, UserId, default)).ReturnsAsync((Tour?)null);

        Assert.ThrowsAsync<TourNotFoundException>(async () =>
            await _service.GetByTourIdAsync(tourId, UserId));
    }

    [Test]
    public async Task GetByIdAsync_ExistingLog_ReturnsLog()
    {
        var log = MockRepositoryFactory.CreateTourLog(userId: UserId);
        _logRepoMock.Setup(r => r.GetByIdAsync(log.Id, UserId, default)).ReturnsAsync(log);

        var result = await _service.GetByIdAsync(log.Id, UserId);

        Assert.That(result.Id, Is.EqualTo(log.Id));
    }

    [Test]
    public void GetByIdAsync_NotFound_ThrowsTourLogNotFoundException()
    {
        var id = Guid.NewGuid();
        _logRepoMock.Setup(r => r.GetByIdAsync(id, UserId, default)).ReturnsAsync((TourLog?)null);

        Assert.ThrowsAsync<TourLogNotFoundException>(async () =>
            await _service.GetByIdAsync(id, UserId));
    }

    [Test]
    public async Task CreateAsync_TourExists_CreatesLog()
    {
        var tour = MockRepositoryFactory.CreateTour(UserId);
        var log = MockRepositoryFactory.CreateTourLog(tour.Id, UserId);
        _tourRepoMock.Setup(r => r.GetByIdAsync(tour.Id, UserId, default)).ReturnsAsync(tour);
        _logRepoMock.Setup(r => r.CreateAsync(log, default)).ReturnsAsync(log);

        var result = await _service.CreateAsync(log);

        Assert.That(result.Id, Is.EqualTo(log.Id));
    }

    [Test]
    public void CreateAsync_TourNotFound_ThrowsTourNotFoundException()
    {
        var log = MockRepositoryFactory.CreateTourLog(userId: UserId);
        _tourRepoMock.Setup(r => r.GetByIdAsync(log.TourId, UserId, default)).ReturnsAsync((Tour?)null);

        Assert.ThrowsAsync<TourNotFoundException>(async () =>
            await _service.CreateAsync(log));
    }

    [Test]
    public async Task UpdateAsync_ExistingLog_UpdatesFields()
    {
        var log = MockRepositoryFactory.CreateTourLog(userId: UserId);
        var updates = new TourLog
        {
            DateTime = DateTime.UtcNow,
            Comment = "Updated comment",
            Difficulty = Models.Enums.Difficulty.Hard,
            TotalDistanceKm = 25.0,
            TotalTimeSeconds = 7200,
            Rating = Models.Enums.Rating.Five
        };
        _logRepoMock.Setup(r => r.GetByIdAsync(log.Id, UserId, default)).ReturnsAsync(log);
        _logRepoMock.Setup(r => r.UpdateAsync(It.IsAny<TourLog>(), default))
            .ReturnsAsync((TourLog l, CancellationToken _) => l);

        var result = await _service.UpdateAsync(log.Id, UserId, updates);

        Assert.That(result.Comment, Is.EqualTo("Updated comment"));
        Assert.That(result.TotalDistanceKm, Is.EqualTo(25.0));
    }

    [Test]
    public void UpdateAsync_NotFound_ThrowsTourLogNotFoundException()
    {
        var id = Guid.NewGuid();
        _logRepoMock.Setup(r => r.GetByIdAsync(id, UserId, default)).ReturnsAsync((TourLog?)null);

        Assert.ThrowsAsync<TourLogNotFoundException>(async () =>
            await _service.UpdateAsync(id, UserId, new TourLog()));
    }

    [Test]
    public async Task DeleteAsync_CallsRepository()
    {
        var id = Guid.NewGuid();
        _logRepoMock.Setup(r => r.DeleteAsync(id, UserId, default)).Returns(Task.CompletedTask);

        await _service.DeleteAsync(id, UserId);

        _logRepoMock.Verify(r => r.DeleteAsync(id, UserId, default), Times.Once);
    }
}
