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
public class ImportExportServiceTests
{
    private Mock<ITourRepository> _tourRepo = null!;
    private Mock<ITourLogRepository> _logRepo = null!;
    private ImportExportService _service = null!;
    private const string UserId = "user-1";

    [SetUp]
    public void SetUp()
    {
        _tourRepo = MockRepositoryFactory.TourRepository();
        _logRepo = MockRepositoryFactory.TourLogRepository();
        _tourRepo.Setup(r => r.CreateAsync(It.IsAny<Tour>(), default))
            .ReturnsAsync((Tour t, CancellationToken _) => t);
        _logRepo.Setup(r => r.CreateAsync(It.IsAny<TourLog>(), default))
            .ReturnsAsync((TourLog l, CancellationToken _) => l);
        _service = new ImportExportService(_tourRepo.Object, _logRepo.Object,
            new Mock<ILogger<ImportExportService>>().Object);
    }

    [Test]
    public async Task ImportAsync_AssignsOwnershipNewIdsAndCreatesLogs()
    {
        var src = MockRepositoryFactory.CreateTour("other-user");
        src.TourLogs = new List<TourLog> { MockRepositoryFactory.CreateTourLog(src.Id, "other-user") };

        var count = await _service.ImportAsync(UserId, new[] { src });

        Assert.That(count, Is.EqualTo(1));
        _tourRepo.Verify(r => r.CreateAsync(It.Is<Tour>(t => t.UserId == UserId && t.Id != src.Id), default), Times.Once);
        _logRepo.Verify(r => r.CreateAsync(It.Is<TourLog>(l => l.UserId == UserId), default), Times.Once);
    }

    [Test]
    public void ImportAsync_EmptyList_ThrowsBusinessLogicException()
    {
        Assert.ThrowsAsync<BusinessLogicException>(async () =>
            await _service.ImportAsync(UserId, Array.Empty<Tour>()));
    }

    [Test]
    public void ImportAsync_MissingRequiredFields_ThrowsBusinessLogicException()
    {
        var bad = new Tour { Name = "", From = "", To = "" };
        Assert.ThrowsAsync<BusinessLogicException>(async () =>
            await _service.ImportAsync(UserId, new[] { bad }));
    }

    [Test]
    public async Task ImportAsync_DuplicateName_AppendsCounterSuffix()
    {
        var src = MockRepositoryFactory.CreateTour(UserId, "Vienna Trip");
        _tourRepo.Setup(r => r.ExistsWithNameAsync(UserId, "Vienna Trip", default)).ReturnsAsync(true);
        _tourRepo.Setup(r => r.ExistsWithNameAsync(UserId, "Vienna Trip (2)", default)).ReturnsAsync(false);

        await _service.ImportAsync(UserId, new[] { src });

        _tourRepo.Verify(r => r.CreateAsync(It.Is<Tour>(t => t.Name == "Vienna Trip (2)"), default), Times.Once);
    }

    [Test]
    public async Task ExportAsync_ReturnsUserTours()
    {
        var tours = new List<Tour> { MockRepositoryFactory.CreateTour(UserId) };
        _tourRepo.Setup(r => r.GetAllAsync(UserId, null, default)).ReturnsAsync(tours);

        var result = await _service.ExportAsync(UserId);

        Assert.That(result.Count(), Is.EqualTo(1));
    }
}
