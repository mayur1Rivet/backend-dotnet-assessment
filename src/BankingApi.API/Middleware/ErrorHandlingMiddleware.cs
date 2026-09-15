using System.Text.Json;
using BankingApi.DTO.Errors;

namespace BankingApi.API.Middleware;

public sealed class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled exception while processing {Path}", context.Request.Path);
            await WriteErrorAsync(context, exception);
        }
    }

    private static async Task WriteErrorAsync(HttpContext context, Exception exception)
    {
        var (statusCode, errorCode, message) = exception switch
        {
            KeyNotFoundException => (StatusCodes.Status404NotFound, "RESOURCE_NOT_FOUND", exception.Message),
            ArgumentException => (StatusCodes.Status400BadRequest, "INVALID_ARGUMENT", exception.Message),
            _ => (StatusCodes.Status500InternalServerError, "INTERNAL_SERVER_ERROR", "An unexpected error occurred.")
        };

        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse(statusCode, errorCode, message, TraceId: context.TraceIdentifier);
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
    }
}