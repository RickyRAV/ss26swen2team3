using TourPlanner.Models;

namespace TourPlanner.BL.Services;

public interface IImportExportService
{
    Task<IEnumerable<Tour>> ExportAsync(string userId, CancellationToken ct = default);

    Task<int> ImportAsync(string userId, IEnumerable<Tour> tours, CancellationToken ct = default);
}
