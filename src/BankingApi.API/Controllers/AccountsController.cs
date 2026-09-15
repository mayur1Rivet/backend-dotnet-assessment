using BankingApi.Command.Accounts;
using BankingApi.DTO.Accounts;
using BankingApi.DTO.Errors;
using BankingApi.Infrastructure.IRepository;
using BankingApi.Query.Accounts;
using BankingApi.Shared.Contracts;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankingApi.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class AccountsController(
    ISender sender,
    IAccountRepository accountRepository,
    IValidator<CreateAccountCommand> createValidator,
    IValidator<UpdateAccountCommand> updateValidator) : ControllerBase
{
    private const string AccountNotFoundCode = "ACCOUNT_NOT_FOUND";

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<AccountResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAllAccountsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("customer/{customerId:int}")]
    public async Task<ActionResult<IEnumerable<AccountResponse>>> GetByCustomerId(int customerId, CancellationToken cancellationToken)
    {
        if (!IsAdmin() && !OwnsCustomer(customerId))
        {
            return Forbid();
        }

        var accounts = await sender.Send(new GetAccountsByCustomerIdQuery(customerId), cancellationToken);
        return Ok(accounts);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AccountResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var account = await sender.Send(new GetAccountByIdQuery(id), cancellationToken);
        if (account is null)
        {
            return NotFound(new ErrorResponse(StatusCodes.Status404NotFound, AccountNotFoundCode, $"Account with id {id} was not found.", TraceId: HttpContext.TraceIdentifier));
        }

        if (!IsAdmin() && !OwnsCustomer(account.CustomerId))
        {
            return Forbid();
        }

        return Ok(account);
    }

    [HttpPost]
    public async Task<ActionResult<AccountResponse>> Create([FromBody] CreateAccountDto dto, CancellationToken cancellationToken)
    {
        if (!IsAdmin() && !OwnsCustomer(dto.CustomerId))
        {
            return Forbid();
        }

        var command = new CreateAccountCommand(dto);
        var validationResult = await createValidator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            var details = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray());

            return BadRequest(new ErrorResponse(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "One or more validation errors occurred.",
                details,
                HttpContext.TraceIdentifier));
        }

        var created = await sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<AccountResponse>> Update(int id, [FromBody] UpdateAccountDto dto, CancellationToken cancellationToken)
    {
        var existingAccount = await accountRepository.GetByIdAsync(id, cancellationToken);
        if (existingAccount is null)
        {
            return NotFound(new ErrorResponse(StatusCodes.Status404NotFound, AccountNotFoundCode, $"Account with id {id} was not found.", TraceId: HttpContext.TraceIdentifier));
        }

        if (!IsAdmin() && !OwnsCustomer(existingAccount.CustomerId))
        {
            return Forbid();
        }

        var command = new UpdateAccountCommand(id, dto);
        var validationResult = await updateValidator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            var details = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray());

            return BadRequest(new ErrorResponse(
                StatusCodes.Status400BadRequest,
                "VALIDATION_ERROR",
                "One or more validation errors occurred.",
                details,
                HttpContext.TraceIdentifier));
        }

        try
        {
            var updated = await sender.Send(command, cancellationToken);
            return Ok(updated);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new ErrorResponse(
                StatusCodes.Status404NotFound,
                AccountNotFoundCode,
                $"Account with id {id} was not found.",
                TraceId: HttpContext.TraceIdentifier));
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var existingAccount = await accountRepository.GetByIdAsync(id, cancellationToken);
        if (existingAccount is null)
        {
            return NotFound(new ErrorResponse(StatusCodes.Status404NotFound, AccountNotFoundCode, $"Account with id {id} was not found.", TraceId: HttpContext.TraceIdentifier));
        }

        if (!IsAdmin() && !OwnsCustomer(existingAccount.CustomerId))
        {
            return Forbid();
        }

        var deleted = await sender.Send(new DeleteAccountCommand(id), cancellationToken);
        if (!deleted)
        {
            return NotFound(new ErrorResponse(
                StatusCodes.Status404NotFound,
                AccountNotFoundCode,
                $"Account with id {id} was not found.",
                TraceId: HttpContext.TraceIdentifier));
        }

        return NoContent();
    }

    private bool IsAdmin() => User.IsInRole("Admin");

    private bool OwnsCustomer(int customerId) =>
        int.TryParse(User.FindFirstValue("customer_id"), out var currentCustomerId) && currentCustomerId == customerId;
}
