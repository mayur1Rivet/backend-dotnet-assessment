using BankingApi.DTO.Accounts;
using BankingApi.Shared.Contracts;

namespace BankingApi.Query.Accounts;

public record GetAccountsByCustomerIdQuery(int CustomerId) : IRequest<IEnumerable<AccountResponse>>;
