using BankingApi.DTO.Customers;
using BankingApi.Infrastructure.Entity;
using BankingApi.Infrastructure.IRepository;
using BankingApi.Shared.Contracts;

namespace BankingApi.Command.Customers;

public class CreateCustomerCommandHandler(ICustomerRepository customerRepository)
    : IRequestHandler<CreateCustomerCommand, CustomerResponse>
{
    public async Task<CustomerResponse> Handle(CreateCustomerCommand request, CancellationToken cancellationToken = default)
    {
        var customer = new Customer
        {
            FirstName = request.Customer.FirstName.Trim(),
            LastName = request.Customer.LastName.Trim(),
            Email = request.Customer.Email.Trim(),
            PhoneNumber = string.IsNullOrWhiteSpace(request.Customer.PhoneNumber)
                ? null
                : request.Customer.PhoneNumber.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        await customerRepository.AddAsync(customer, cancellationToken);
        await customerRepository.SaveChangesAsync(cancellationToken);

        return new CustomerResponse
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email,
            PhoneNumber = customer.PhoneNumber,
            CreatedAt = customer.CreatedAt
        };
    }
}
