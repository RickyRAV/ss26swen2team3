namespace TourPlanner.Api.DTOs;

public record RegisterRequest(string Email, string UserName, string Password);
public record LoginRequest(string Email, string Password);

public record AuthResponse(
    string AccessToken,
    string UserId,
    string Email,
    string UserName
);
