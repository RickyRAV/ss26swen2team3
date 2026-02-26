namespace TourPlanner.BL.Services;

public interface IImageService
{
    Task<string> SaveImageAsync(Guid tourId, Stream imageStream, string fileName, CancellationToken ct = default);

    void DeleteImage(string? imagePath);
}
