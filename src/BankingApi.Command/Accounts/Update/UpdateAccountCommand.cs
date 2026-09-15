using BankingApi.DTO.Accounts;
using BankingApi.Shared.Contracts;

namespace BankingApi.Command.Accounts;

public record UpdateAccountCommand(int Id, UpdateAccountDto Account) : IRequest<AccountResponse>;
