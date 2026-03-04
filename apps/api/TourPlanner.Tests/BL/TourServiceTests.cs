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
public class TourServiceTests
{
    private Mock<ITourRepository> _repoMock = null!;
    private Mock<ILogger<TourService>> _loggerMock = null!;
    private TourService _service = null!;
    private const string UserId = "user-1";

    [SetUp]
    public void SetUp()
    {
        _repoMock = MockRepositoryFactory.TourRepository();
        _loggerMock = new Mock<ILogger<TourService>>();
        _service = new TourService(_repoMock.Object, _loggerMock.Object);
    }

    [Test]
    public async Task GetAllAsync_ReturnsToursForUser()
    {
        var tours = new List<Tour> { MockRepositoryFactory.CreateTour(UserId) };
        _repoMock.Setup(r => r.GetAllAsync(UserId, null, default)).ReturnsAsync(tours);

        var result = await _service.GetAllAsync(UserId);

        Assert.That(result.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task GetAllAsync_WithSearchQuery_PassesQueryToRepository()
    {
        var query = "Vienna";
        _repoMock.Setup(r => r.GetAllAsync(UserId, query, default)).ReturnsAsync(new List<Tour>());

        await _service.GetAllAsync(UserId, query);

        _repoMock.Verify(r => r.GetAllAsync(UserId, query, default), Times.Once);
    }

    [Test]
    public async Task GetByIdAsync_ExistingTour_ReturnsTour()
    {
        var tour = MockRepositoryFactory.CreateTour(UserId);
        _repoMock.Setup(r => r.GetByIdAsync(tour.Id, UserId, default)).ReturnsAsync(tour);

        var result = await _service.GetByIdAsync(tour.Id, UserId);

        Assert.That(result.Id, Is.EqualTo(tour.Id));
    }

    [Test]
    public void GetByIdAsync_NotFound_ThrowsTourNotFoundException()
    {
        var id = Guid.NewGuid();
        _repoMock.Setup(r => r.GetByIdAsync(id, UserId, default)).ReturnsAsync((Tour?)null);

        Assert.ThrowsAsync<TourNotFoundException>(async () =>
            await _service.GetByIdAsync(id, UserId));
    }

    [Test]
    public async Task CreateAsync_ValidTour_CallsRepository()
    {
        var tour = MockRepositoryFactory.CreateTour(UserId);
        _repoMock.Setup(r => r.CreateAsync(tour, default)).ReturnsAsync(tour);

        var result = await _service.CreateAsync(tour);

        _repoMock.Verify(r => r.CreateAsync(tour, default), Times.Once);
        Assert.That(result.Id, Is.EqualTo(tour.Id));
    }

    [Test]
    public async Task UpdateAsync_ExistingTour_UpdatesFields()
    {
        var tour = MockRepositoryFactory.CreateTour(UserId);
        var updates = new Tour { Name = "Updated", Description = "New desc", From = "Linz", To = "Salzburg", TransportType = Models.Enums.TransportType.Bicycle };
        _repoMock.Setup(r => r.GetByIdAsync(tour.Id, UserId, default)).ReturnsAsync(tour);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Tour>(), default)).ReturnsAsync((Tour t, CancellationToken _) => t);

        var result = await _service.UpdateAsync(tour.Id, UserId, updates);

        Assert.That(result.Name, Is.EqualTo("Updated"));
        Assert.That(result.From, Is.EqualTo("Linz"));
    }

    [Test]
    public void UpdateAsync_NotFound_ThrowsTourNotFoundException()
    {
        var id = Guid.NewGuid();
        _repoMock.Setup(r => r.GetByIdAsync(id, UserId, default)).ReturnsAsync((Tour?)null);

        Assert.ThrowsAsync<TourNotFoundException>(async () =>
            await _service.UpdateAsync(id, UserId, new Tour()));
    }

    [Test]
    public async Task DeleteAsync_CallsRepository()
    {
        var id = Guid.NewGuid();
        _repoMock.Setup(r => r.DeleteAsync(id, UserId, default)).Returns(Task.CompletedTask);

        await _service.DeleteAsync(id, UserId);

        _repoMock.Verify(r => r.DeleteAsync(id, UserId, default), Times.Once);
    }
}
