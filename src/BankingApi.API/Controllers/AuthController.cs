using BankingApi.Command.Auth;
using BankingApi.DTO.Auth;
using BankingApi.Shared.Contracts;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingApi.API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(
    ISender sender,
    IValidator<RegisterCommand> registerValidator,
    IValidator<LoginCommand> loginValidator) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var command = new RegisterCommand(request);
        var validationResult = await registerValidator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(new
            {
                statusCode = StatusCodes.Status400BadRequest,
                errorCode = "VALIDATION_ERROR",
                message = "One or more validation errors occurred.",
                details = validationResult.Errors.GroupBy(error => error.PropertyName)
                    .ToDictionary(group => group.Key, group => group.Select(error => error.ErrorMessage).ToArray()),
                traceId = HttpContext.TraceIdentifier
            });
        }

        return Ok(await sender.Send(command, cancellationToken));
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var command = new LoginCommand(request);
        var validationResult = await loginValidator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(new
            {
                statusCode = StatusCodes.Status400BadRequest,
                errorCode = "VALIDATION_ERROR",
                message = "One or more validation errors occurred.",
                details = validationResult.Errors.GroupBy(error => error.PropertyName)
                    .ToDictionary(group => group.Key, group => group.Select(error => error.ErrorMessage).ToArray()),
                traceId = HttpContext.TraceIdentifier
            });
        }

        return Ok(await sender.Send(command, cancellationToken));
    }
}