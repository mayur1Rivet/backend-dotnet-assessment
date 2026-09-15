using BankingApi.Command.Customers;
using BankingApi.DTO.Customers;
using BankingApi.Query.Customers;
using BankingApi.Shared.Contracts;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace BankingApi.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController(ISender sender, IValidator<CreateCustomerCommand> createValidator, IValidator<UpdateCustomerCommand> updateValidator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAllCustomersQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CustomerResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var customer = await sender.Send(new GetCustomerByIdQuery(id), cancellationToken);
        if (customer is null)
        {
            return NotFound();
        }

        return Ok(customer);
    }

    [HttpPost]
    public async Task<ActionResult<CustomerResponse>> Create([FromBody] CreateCustomerDto dto, CancellationToken cancellationToken)
    {
        var command = new CreateCustomerCommand(dto);
        var validationResult = await createValidator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            var problem = new ValidationProblemDetails(
                validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()));

            return ValidationProblem(problem);
        }

        var created = await sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CustomerResponse>> Update(int id, [FromBody] UpdateCustomerDto dto, CancellationToken cancellationToken)
    {
        var command = new UpdateCustomerCommand(id, dto);
        var validationResult = await updateValidator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            var problem = new ValidationProblemDetails(
                validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()));

            return ValidationProblem(problem);
        }

        try
        {
            var updated = await sender.Send(command, cancellationToken);
            return Ok(updated);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await sender.Send(new DeleteCustomerCommand(id), cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
