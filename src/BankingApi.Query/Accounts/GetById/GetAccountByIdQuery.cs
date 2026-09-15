using BankingApi.DTO.Accounts;
using BankingApi.Shared.Contracts;

namespace BankingApi.Query.Accounts;

public record GetAccountByIdQuery(int Id) : IRequest<AccountResponse?>;
