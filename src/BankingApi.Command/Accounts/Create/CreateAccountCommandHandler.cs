using BankingApi.DTO.Accounts;
using BankingApi.Infrastructure.Entity;
using BankingApi.Infrastructure.IRepository;
using BankingApi.Shared.Contracts;

namespace BankingApi.Command.Accounts;

public class CreateAccountCommandHandler(IAccountRepository accountRepository)
    : IRequestHandler<CreateAccountCommand, AccountResponse>
{
    public async Task<AccountResponse> Handle(CreateAccountCommand request, CancellationToken cancellationToken = default)
    {
        var account = new Account
        {
            CustomerId = request.Account.CustomerId,
            AccountType = request.Account.AccountType.Trim(),
            Balance = request.Account.Balance,
            AccountNumber = GenerateAccountNumber(),
            CreatedAt = DateTime.UtcNow
        };

        await accountRepository.AddAsync(account, cancellationToken);
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

    private static string GenerateAccountNumber()
    {
        var random = new Random();
        return $"ACC-{random.Next(100000, 999999)}";
    }
}
