using BankingApi.DTO.Accounts;
using BankingApi.Shared.Contracts;

namespace BankingApi.Query.Accounts;

public record GetAllAccountsQuery : IRequest<IEnumerable<AccountResponse>>;
