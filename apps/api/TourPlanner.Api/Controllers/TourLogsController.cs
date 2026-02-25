using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourPlanner.Api.DTOs;
using TourPlanner.BL.Services;
using TourPlanner.Models;

namespace TourPlanner.Api.Controllers;

[ApiController]
[Route("api/v1/tours/{tourId:guid}/logs")]
[Authorize]
public class TourLogsController : ControllerBase
{
    private readonly ITourLogService _tourLogService;

    public TourLogsController(ITourLogService tourLogService)
    {
        _tourLogService = tourLogService;
    }

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)
        ?? throw new UnauthorizedAccessException("User ID not found in token.");

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TourLogDto>>> GetAll(Guid tourId, CancellationToken ct)
    {
        var logs = await _tourLogService.GetByTourIdAsync(tourId, UserId, ct);
        return Ok(logs.Select(MapToDto));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TourLogDto>> GetById(Guid tourId, Guid id, CancellationToken ct)
    {
        var log = await _tourLogService.GetByIdAsync(id, UserId, ct);
        return Ok(MapToDto(log));
    }

    [HttpPost]
    public async Task<ActionResult<TourLogDto>> Create(Guid tourId, [FromBody] CreateTourLogRequest request, CancellationToken ct)
    {
        var log = new TourLog
        {
            TourId = tourId,
            UserId = UserId,
            DateTime = request.DateTime,
            Comment = request.Comment,
            Difficulty = request.Difficulty,
            TotalDistanceKm = request.TotalDistanceKm,
            TotalTimeSeconds = request.TotalTimeSeconds,
            Rating = request.Rating
        };

        var created = await _tourLogService.CreateAsync(log, ct);
        return CreatedAtAction(nameof(GetById), new { tourId, id = created.Id }, MapToDto(created));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TourLogDto>> Update(Guid tourId, Guid id, [FromBody] UpdateTourLogRequest request, CancellationToken ct)
    {
        var updates = new TourLog
        {
            DateTime = request.DateTime,
            Comment = request.Comment,
            Difficulty = request.Difficulty,
            TotalDistanceKm = request.TotalDistanceKm,
            TotalTimeSeconds = request.TotalTimeSeconds,
            Rating = request.Rating
        };

        var updated = await _tourLogService.UpdateAsync(id, UserId, updates, ct);
        return Ok(MapToDto(updated));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid tourId, Guid id, CancellationToken ct)
    {
        await _tourLogService.DeleteAsync(id, UserId, ct);
        return NoContent();
    }

    private static TourLogDto MapToDto(TourLog l) => new(
        l.Id, l.TourId, l.DateTime, l.Comment, l.Difficulty,
        l.TotalDistanceKm, l.TotalTimeSeconds, l.Rating,
        l.CreatedAt, l.UpdatedAt
    );
}
