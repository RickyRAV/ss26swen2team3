using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TourPlanner.BL.Services;

public class ImageServiceOptions
{
    public string BasePath { get; set; } = "images";
}

public class ImageService : IImageService
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp"
    };

    private readonly string _basePath;
    private readonly ILogger<ImageService> _logger;

    public ImageService(IOptions<ImageServiceOptions> options, ILogger<ImageService> logger)
    {
        _basePath = options.Value.BasePath;
        _logger = logger;
        Directory.CreateDirectory(_basePath);
    }

    public async Task<string> SaveImageAsync(Guid tourId, Stream imageStream, string fileName, CancellationToken ct = default)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();

        if (!AllowedExtensions.Contains(ext))
            throw new ArgumentException($"Unsupported image format '{ext}'. Allowed: jpg, png, webp.");

        // One image per tour — overwrite previous if it exists
        var relativeName = $"{tourId}{ext}";
        var fullPath = Path.Combine(_basePath, relativeName);

        _logger.LogInformation("Saving tour image to {Path}", fullPath);

        await using var file = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None);
        await imageStream.CopyToAsync(file, ct);

        return relativeName;
    }

    public void DeleteImage(string? imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath)) return;

        var fullPath = Path.Combine(_basePath, imagePath);
        if (!File.Exists(fullPath)) return;

        _logger.LogInformation("Deleting tour image {Path}", fullPath);
        File.Delete(fullPath);
    }
}
