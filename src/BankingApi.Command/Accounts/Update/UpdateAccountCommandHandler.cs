using BankingApi.DTO.Accounts;
using BankingApi.Infrastructure.IRepository;
using BankingApi.Shared.Contracts;

namespace BankingApi.Command.Accounts;

public class UpdateAccountCommandHandler(IAccountRepository accountRepository)
    : IRequestHandler<UpdateAccountCommand, AccountResponse>
{
    public async Task<AccountResponse> Handle(UpdateAccountCommand request, CancellationToken cancellationToken = default)
    {
        var account = await accountRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Account with id {request.Id} was not found.");

        account.AccountType = request.Account.AccountType.Trim();
        account.Balance = request.Account.Balance;

        await accountRepository.UpdateAsync(account, cancellationToken);
        await accountRepository.SaveChangesAsync(cancellationToken);

        return new AccountResponse
        {
            Id = account.Id,
            CustomerId = account.CustomerId,
            AccountNumber = account.AccountNumber,
            AccountType = account.AccountType,
            Balance = account.Balance,
            CreatedAt = account.CreatedAt
        };
    }
}
