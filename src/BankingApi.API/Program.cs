using BankingApi.API.Middleware;
using BankingApi.DTO.Errors;
using BankingApi.AppService.Extensions;
using BankingApi.Infrastructure.Extensions;
using BankingApi.Shared.Extensions;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddSharedServices();
builder.Services.AddAppServices();
builder.Services.AddControllers()
.ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var details = context.ModelState
            .Where(entry => entry.Value?.Errors.Count > 0)
            .ToDictionary(
                entry => entry.Key,
                entry => entry.Value!.Errors.Select(error => error.ErrorMessage).ToArray());

        return new BadRequestObjectResult(new ErrorResponse(
            StatusCodes.Status400BadRequest,
            "VALIDATION_ERROR",
            "One or more validation errors occurred.",
            details,
            context.HttpContext.TraceIdentifier));
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseStatusCodePages(async statusCodeContext =>
{
    var response = statusCodeContext.HttpContext.Response;
    if (response.StatusCode < StatusCodes.Status400BadRequest || response.HasStarted)
    {
        return;
    }

    var (errorCode, message) = response.StatusCode switch
    {
        StatusCodes.Status401Unauthorized => ("UNAUTHORIZED", "Authentication is required."),
        StatusCodes.Status403Forbidden => ("FORBIDDEN", "You are not authorized to perform this action."),
        StatusCodes.Status404NotFound => ("ROUTE_NOT_FOUND", "The requested resource was not found."),
        StatusCodes.Status405MethodNotAllowed => ("METHOD_NOT_ALLOWED", "The HTTP method is not allowed for this resource."),
        _ => ("HTTP_ERROR", "The request could not be completed.")
    };

    response.ContentType = "application/json";
    await response.WriteAsJsonAsync(new ErrorResponse(
        response.StatusCode,
        errorCode,
        message,
        TraceId: statusCodeContext.HttpContext.TraceIdentifier));
});

app.UseMiddleware<ErrorHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "BankingApi v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
