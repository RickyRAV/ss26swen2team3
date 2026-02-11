using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TourPlanner.BL.Exceptions;
using TourPlanner.DAL.Repositories;
using TourPlanner.Models;

namespace TourPlanner.BL.Services;

public class JwtSettings
{
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int AccessTokenExpiryMinutes { get; set; } = 15;
    public int RefreshTokenExpiryDays { get; set; } = 7;
}

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        IRefreshTokenRepository refreshTokenRepository,
        IOptions<JwtSettings> jwtSettings,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtSettings = jwtSettings.Value;
        _logger = logger;
    }

    public async Task<AuthResult> RegisterAsync(string email, string userName, string password, CancellationToken ct = default)
    {
        var existing = await _userManager.FindByEmailAsync(email);
        if (existing is not null)
            throw new AuthException("An account with this email already exists.");

        var user = new ApplicationUser { Email = email, UserName = userName };
        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            _logger.LogWarning("Registration failed for {Email}: {Errors}", email, errors);
            throw new AuthException($"Registration failed: {errors}");
        }

        _logger.LogInformation("User {Email} registered successfully", email);
        var accessToken = GenerateAccessToken(user);
        return new AuthResult(accessToken, user.Id, user.Email!, user.UserName!);
    }

    public async Task<AuthResult> LoginAsync(string email, string password, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null || !await _userManager.CheckPasswordAsync(user, password))
        {
            _logger.LogWarning("Login failed for {Email}", email);
            throw new AuthException("Invalid email or password.");
        }

        _logger.LogInformation("User {Email} logged in", email);
        var accessToken = GenerateAccessToken(user);
        return new AuthResult(accessToken, user.Id, user.Email!, user.UserName!);
    }

    public async Task<AuthResult> RefreshAsync(string refreshToken, CancellationToken ct = default)
    {
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken, ct);
        if (storedToken is null)
            throw new AuthException("Invalid or expired refresh token.");

        var user = await _userManager.FindByIdAsync(storedToken.UserId);
        if (user is null)
            throw new AuthException("User no longer exists.");

        await _refreshTokenRepository.RevokeAsync(refreshToken, ct);

        var accessToken = GenerateAccessToken(user);
        return new AuthResult(accessToken, user.Id, user.Email!, user.UserName!);
    }

    public async Task LogoutAsync(string refreshToken, CancellationToken ct = default)
    {
        await _refreshTokenRepository.RevokeAsync(refreshToken, ct);
        _logger.LogInformation("Refresh token revoked");
    }

    public async Task<string> GenerateRefreshTokenAsync(string userId, CancellationToken ct = default)
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var refreshToken = new RefreshToken
        {
            Token = token,
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays)
        };
        await _refreshTokenRepository.CreateAsync(refreshToken, ct);
        return token;
    }

    private string GenerateAccessToken(ApplicationUser user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
