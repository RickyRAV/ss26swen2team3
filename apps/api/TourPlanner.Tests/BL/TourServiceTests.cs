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
    private Mock<IOrsService> _orsMock = null!;
    private Mock<ILogger<TourService>> _loggerMock = null!;
    private TourService _service = null!;
    private const string UserId = "user-1";

    [SetUp]
    public void SetUp()
    {
        _repoMock = MockRepositoryFactory.TourRepository();
        _orsMock = MockRepositoryFactory.OrsService();
        _loggerMock = new Mock<ILogger<TourService>>();
        _service = new TourService(_repoMock.Object, _orsMock.Object, _loggerMock.Object);
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
    public async Task GetAllAsync_ForwardsSearchQueryToRepository()
    {
        _repoMock.Setup(r => r.GetAllAsync(UserId, "sunset", default))
            .ReturnsAsync(new List<Tour> { MockRepositoryFactory.CreateTour(UserId) });

        var result = await _service.GetAllAsync(UserId, "sunset");

        Assert.That(result.Count(), Is.EqualTo(1));
        _repoMock.Verify(r => r.GetAllAsync(UserId, "sunset", default), Times.Once);
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
    public void CreateAsync_DuplicateName_ThrowsAndDoesNotPersist()
    {
        var tour = MockRepositoryFactory.CreateTour(UserId);
        _repoMock.Setup(r => r.ExistsWithNameAsync(tour.UserId, tour.Name, default)).ReturnsAsync(true);

        Assert.ThrowsAsync<BusinessLogicException>(async () => await _service.CreateAsync(tour));

        // The uniqueness guard must reject before any ORS call or persistence.
        _orsMock.Verify(o => o.GetRouteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _repoMock.Verify(r => r.CreateAsync(It.IsAny<Tour>(), default), Times.Never);
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

    [Test]
    public async Task CreateAsync_PersistsOrsComputedRouteData()
    {
        var tour = MockRepositoryFactory.CreateTour(UserId);
        _repoMock.Setup(r => r.CreateAsync(It.IsAny<Tour>(), default))
            .ReturnsAsync((Tour t, CancellationToken _) => t);

        var result = await _service.CreateAsync(tour);

        Assert.That(result.Distance, Is.EqualTo(50.0));          // 50000 m -> 50 km
        Assert.That(result.EstimatedTimeSeconds, Is.EqualTo(9000));
        Assert.That(result.RouteInformation, Does.Contain("coordinates"));
        Assert.That(result.RouteInformation, Does.Contain("elevations"));
        _orsMock.Verify(o => o.GetRouteAsync(tour.From, tour.To, It.IsAny<string>(), default), Times.Once);
    }

    [Test]
    public async Task CreateAsync_OrsFailure_StillCreatesTourWithoutRoute()
    {
        var tour = MockRepositoryFactory.CreateTour(UserId);
        _orsMock.Setup(o => o.GetRouteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("ORS unavailable"));
        _repoMock.Setup(r => r.CreateAsync(It.IsAny<Tour>(), default))
            .ReturnsAsync((Tour t, CancellationToken _) => t);

        var result = await _service.CreateAsync(tour);

        Assert.That(result.Distance, Is.Null);
        Assert.That(result.RouteInformation, Is.Null);
        _repoMock.Verify(r => r.CreateAsync(It.IsAny<Tour>(), default), Times.Once);
    }

    [Test]
    public async Task UpdateAsync_RouteInputsUnchanged_DoesNotRecomputeRoute()
    {
        var tour = MockRepositoryFactory.CreateTour(UserId);
        var updates = new Tour { Name = "Renamed", Description = "new", From = tour.From, To = tour.To, TransportType = tour.TransportType };
        _repoMock.Setup(r => r.GetByIdAsync(tour.Id, UserId, default)).ReturnsAsync(tour);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Tour>(), default)).ReturnsAsync((Tour t, CancellationToken _) => t);

        await _service.UpdateAsync(tour.Id, UserId, updates);

        _orsMock.Verify(o => o.GetRouteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task UpdateAsync_DestinationChanged_RecomputesRoute()
    {
        var tour = MockRepositoryFactory.CreateTour(UserId);
        var updates = new Tour { Name = tour.Name, Description = tour.Description, From = tour.From, To = "Salzburg", TransportType = tour.TransportType };
        _repoMock.Setup(r => r.GetByIdAsync(tour.Id, UserId, default)).ReturnsAsync(tour);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Tour>(), default)).ReturnsAsync((Tour t, CancellationToken _) => t);

        var result = await _service.UpdateAsync(tour.Id, UserId, updates);

        _orsMock.Verify(o => o.GetRouteAsync(tour.From, "Salzburg", It.IsAny<string>(), default), Times.Once);
        Assert.That(result.Distance, Is.EqualTo(50.0));
    }
}
