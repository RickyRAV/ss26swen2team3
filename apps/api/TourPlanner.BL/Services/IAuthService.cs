namespace TourPlanner.BL.Services;

public record AuthResult(string AccessToken, string UserId, string Email, string UserName);

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(string email, string userName, string password, CancellationToken ct = default);
    Task<AuthResult> LoginAsync(string email, string password, CancellationToken ct = default);
    Task<AuthResult> RefreshAsync(string refreshToken, CancellationToken ct = default);
    Task LogoutAsync(string refreshToken, CancellationToken ct = default);
    Task<string> GenerateRefreshTokenAsync(string userId, CancellationToken ct = default);
}
