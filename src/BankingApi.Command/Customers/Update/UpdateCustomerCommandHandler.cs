using BankingApi.DTO.Customers;
using BankingApi.Infrastructure.IRepository;
using BankingApi.Shared.Contracts;

namespace BankingApi.Command.Customers;

public class UpdateCustomerCommandHandler(ICustomerRepository customerRepository)
    : IRequestHandler<UpdateCustomerCommand, CustomerResponse>
{
    public async Task<CustomerResponse> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken = default)
    {
        var existingCustomer = await customerRepository.GetByIdAsync(request.Id, cancellationToken);
        if (existingCustomer is null)
        {
            throw new KeyNotFoundException($"Customer with id {request.Id} was not found.");
        }

        existingCustomer.FirstName = request.Customer.FirstName.Trim();
        existingCustomer.LastName = request.Customer.LastName.Trim();
        existingCustomer.Email = request.Customer.Email.Trim();
        existingCustomer.PhoneNumber = string.IsNullOrWhiteSpace(request.Customer.PhoneNumber)
            ? null
            : request.Customer.PhoneNumber.Trim();

        await customerRepository.UpdateAsync(existingCustomer, cancellationToken);
        await customerRepository.SaveChangesAsync(cancellationToken);

        return new CustomerResponse
        {
            Id = existingCustomer.Id,
            FirstName = existingCustomer.FirstName,
            LastName = existingCustomer.LastName,
            Email = existingCustomer.Email,
            PhoneNumber = existingCustomer.PhoneNumber,
            CreatedAt = existingCustomer.CreatedAt
        };
    }
}
