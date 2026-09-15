using BankingApi.Shared.Contracts;

namespace BankingApi.Command.Accounts;

public record DeleteAccountCommand(int Id) : IRequest<bool>;
