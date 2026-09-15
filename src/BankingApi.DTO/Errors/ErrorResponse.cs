namespace BankingApi.DTO.Errors;

public sealed record ErrorResponse(
    int StatusCode,
    string ErrorCode,
    string Message,
    object? Details = null,
    string? TraceId = null);