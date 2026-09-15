using BankingApi.DTO.Accounts;
using BankingApi.Infrastructure.IRepository;
using BankingApi.Shared.Contracts;

namespace BankingApi.Query.Accounts;

public class GetAccountByIdQueryHandler(IAccountRepository accountRepository)
    : IRequestHandler<GetAccountByIdQuery, AccountResponse?>
{
    public async Task<AccountResponse?> Handle(GetAccountByIdQuery request, CancellationToken cancellationToken = default)
    {
        var account = await accountRepository.GetByIdAsync(request.Id, cancellationToken);
        if (account is null)
        {
            return null;
        }

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
