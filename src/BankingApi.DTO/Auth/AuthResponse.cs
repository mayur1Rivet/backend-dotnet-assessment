namespace BankingApi.DTO.Auth;

public sealed record AuthResponse(
    string AccessToken,
    DateTime ExpiresAt,
    int UserId,
    int CustomerId,
    string Role);