using BankingApi.Command.Customers;
using BankingApi.DTO.Customers;
using BankingApi.DTO.Errors;
using BankingApi.Infrastructure.IRepository;
using BankingApi.Query.Customers;
using BankingApi.Shared.Contracts;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankingApi.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class CustomersController(
    ISender sender,
    ICustomerRepository customerRepository,
    IValidator<CreateCustomerCommand> createValidator,
    IValidator<UpdateCustomerCommand> updateValidator) : ControllerBase
{
    private const string CustomerNotFoundCode = "CUSTOMER_NOT_FOUND";

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<CustomerResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAllCustomersQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CustomerResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        if (!IsAdmin() && !OwnsCustomer(id))
        {
            return Forbid();
        }

        var customer = await sender.Send(new GetCustomerByIdQuery(id), cancellationToken);
        if (customer is null)
        {
            return NotFound(new ErrorResponse(
                StatusCodes.Status404NotFound,
                CustomerNotFoundCode,
                $"Customer with id {id} was not found.",
                TraceId: HttpContext.TraceIdentifier));
        }

        return Ok(customer);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CustomerResponse>> Create([FromBody] CreateCustomerDto dto, CancellationToken cancellationToken)
    {
        var command = new CreateCustomerCommand(dto);
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
    public async Task<ActionResult<CustomerResponse>> Update(int id, [FromBody] UpdateCustomerDto dto, CancellationToken cancellationToken)
    {
        if (await customerRepository.GetByIdAsync(id, cancellationToken) is null)
        {
            return NotFound(new ErrorResponse(StatusCodes.Status404NotFound, CustomerNotFoundCode, $"Customer with id {id} was not found.", TraceId: HttpContext.TraceIdentifier));
        }

        if (!IsAdmin() && !OwnsCustomer(id))
        {
            return Forbid();
        }

        var command = new UpdateCustomerCommand(id, dto);
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
                CustomerNotFoundCode,
                $"Customer with id {id} was not found.",
                TraceId: HttpContext.TraceIdentifier));
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        if (!IsAdmin() && !OwnsCustomer(id))
        {
            return Forbid();
        }

        var deleted = await sender.Send(new DeleteCustomerCommand(id), cancellationToken);
        if (!deleted)
        {
            return NotFound(new ErrorResponse(
                StatusCodes.Status404NotFound,
                CustomerNotFoundCode,
                $"Customer with id {id} was not found.",
                TraceId: HttpContext.TraceIdentifier));
        }

        return NoContent();
    }

    private bool IsAdmin() => User.IsInRole("Admin");

    private bool OwnsCustomer(int customerId) =>
        int.TryParse(User.FindFirstValue("customer_id"), out var currentCustomerId) && currentCustomerId == customerId;
}
