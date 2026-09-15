using BankingApi.DTO.Accounts;
using BankingApi.Shared.Contracts;

namespace BankingApi.Command.Accounts;

public record CreateAccountCommand(CreateAccountDto Account) : IRequest<AccountResponse>;
