using BankingApi.DTO.Customers;
using BankingApi.Shared.Contracts;

namespace BankingApi.Query.Customers;

public record GetAllCustomersQuery : IRequest<IEnumerable<CustomerResponse>>;
