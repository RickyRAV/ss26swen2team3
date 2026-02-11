using Microsoft.AspNetCore.Mvc;
using TourPlanner.Api.DTOs;
using TourPlanner.BL.Services;

namespace TourPlanner.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var result = await _authService.RegisterAsync(request.Email, request.UserName, request.Password, ct);
        var refreshToken = await _authService.GenerateRefreshTokenAsync(result.UserId, ct);
        SetRefreshTokenCookie(refreshToken);
        return Ok(new AuthResponse(result.AccessToken, result.UserId, result.Email, result.UserName));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await _authService.LoginAsync(request.Email, request.Password, ct);
        var refreshToken = await _authService.GenerateRefreshTokenAsync(result.UserId, ct);
        SetRefreshTokenCookie(refreshToken);
        return Ok(new AuthResponse(result.AccessToken, result.UserId, result.Email, result.UserName));
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh(CancellationToken ct)
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
            return Unauthorized(new { detail = "No refresh token provided." });

        var result = await _authService.RefreshAsync(refreshToken, ct);
        var newRefreshToken = await _authService.GenerateRefreshTokenAsync(result.UserId, ct);
        SetRefreshTokenCookie(newRefreshToken);
        return Ok(new AuthResponse(result.AccessToken, result.UserId, result.Email, result.UserName));
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (!string.IsNullOrEmpty(refreshToken))
            await _authService.LogoutAsync(refreshToken, ct);

        Response.Cookies.Delete("refreshToken");
        return NoContent();
    }

    private void SetRefreshTokenCookie(string token)
    {
        Response.Cookies.Append("refreshToken", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = !HttpContext.Request.Host.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase),
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });
    }
}
