using BankingApi.DTO.Accounts;
using BankingApi.Infrastructure.Entity;
using BankingApi.Infrastructure.IRepository;
using BankingApi.Shared.Contracts;
using System.Security.Cryptography;

namespace BankingApi.Command.Accounts;

public class CreateAccountCommandHandler(
    IAccountRepository accountRepository,
    ICustomerRepository customerRepository)
    : IRequestHandler<CreateAccountCommand, AccountResponse>
{
    public async Task<AccountResponse> Handle(CreateAccountCommand request, CancellationToken cancellationToken = default)
    {
        if (await customerRepository.GetByIdAsync(request.Account.CustomerId, cancellationToken) is null)
        {
            throw new KeyNotFoundException($"Customer with id {request.Account.CustomerId} was not found.");
        }

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
        return $"ACC-{RandomNumberGenerator.GetInt32(100000, 1000000)}";
    }
}
