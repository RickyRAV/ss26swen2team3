using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourPlanner.Api.DTOs;
using TourPlanner.BL.Services;
using TourPlanner.Models;
using TourPlanner.Models.Enums;

namespace TourPlanner.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ToursController : ControllerBase
{
    private readonly ITourService _tourService;
    private readonly IImageService _imageService;

    public ToursController(ITourService tourService, IImageService imageService)
    {
        _tourService = tourService;
        _imageService = imageService;
    }

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)
        ?? throw new UnauthorizedAccessException("User ID not found in token.");

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TourDto>>> GetAll([FromQuery] string? q, CancellationToken ct)
    {
        var tours = await _tourService.GetAllAsync(UserId, q, ct);
        return Ok(tours.Select(MapToDto));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TourDto>> GetById(Guid id, CancellationToken ct)
    {
        var tour = await _tourService.GetByIdAsync(id, UserId, ct);
        return Ok(MapToDto(tour));
    }

    [HttpPost]
    public async Task<ActionResult<TourDto>> Create([FromBody] CreateTourRequest request, CancellationToken ct)
    {
        var tour = new Tour
        {
            UserId = UserId,
            Name = request.Name,
            Description = request.Description,
            From = request.From,
            To = request.To,
            TransportType = request.TransportType
        };

        var created = await _tourService.CreateAsync(tour, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, MapToDto(created));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TourDto>> Update(Guid id, [FromBody] UpdateTourRequest request, CancellationToken ct)
    {
        var updates = new Tour
        {
            Name = request.Name,
            Description = request.Description,
            From = request.From,
            To = request.To,
            TransportType = request.TransportType,
            Distance = request.Distance,
            EstimatedTimeSeconds = request.EstimatedTimeSeconds,
            RouteInformation = request.RouteInformation
        };

        var updated = await _tourService.UpdateAsync(id, UserId, updates, ct);
        return Ok(MapToDto(updated));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _tourService.DeleteAsync(id, UserId, ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/image")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<ActionResult<TourDto>> UploadImage(Guid id, IFormFile file, CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return BadRequest("No file provided.");

        var tour = await _tourService.GetByIdAsync(id, UserId, ct);

        _imageService.DeleteImage(tour.ImagePath);

        var relativeName = await _imageService.SaveImageAsync(id, file.OpenReadStream(), file.FileName, ct);

        var updates = new Tour
        {
            Name = tour.Name,
            Description = tour.Description,
            From = tour.From,
            To = tour.To,
            TransportType = tour.TransportType,
            Distance = tour.Distance,
            EstimatedTimeSeconds = tour.EstimatedTimeSeconds,
            RouteInformation = tour.RouteInformation,
            ImagePath = relativeName,
        };

        var updated = await _tourService.UpdateAsync(id, UserId, updates, ct);
        return Ok(MapToDto(updated));
    }

    private static TourDto MapToDto(Tour t) => new(
        t.Id, t.Name, t.Description, t.From, t.To, t.TransportType,
        t.Distance, t.EstimatedTimeSeconds, t.RouteInformation, t.ImagePath,
        t.Popularity, t.ChildFriendliness.ToString(),
        t.CreatedAt, t.UpdatedAt
    );
}
