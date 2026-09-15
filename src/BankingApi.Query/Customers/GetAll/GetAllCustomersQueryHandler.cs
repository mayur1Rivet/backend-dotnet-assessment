using BankingApi.DTO.Customers;
using BankingApi.Infrastructure.IRepository;
using BankingApi.Shared.Contracts;

namespace BankingApi.Query.Customers;

public class GetAllCustomersQueryHandler(ICustomerRepository customerRepository)
    : IRequestHandler<GetAllCustomersQuery, IEnumerable<CustomerResponse>>
{
    public async Task<IEnumerable<CustomerResponse>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken = default)
    {
        var customers = await customerRepository.GetAllAsync(cancellationToken);

        return customers.Select(c => new CustomerResponse
        {
            Id = c.Id,
            FirstName = c.FirstName,
            LastName = c.LastName,
            Email = c.Email,
            PhoneNumber = c.PhoneNumber,
            CreatedAt = c.CreatedAt
        });
    }
}
