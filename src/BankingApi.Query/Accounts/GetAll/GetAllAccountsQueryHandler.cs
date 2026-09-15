using BankingApi.DTO.Accounts;
using BankingApi.Infrastructure.IRepository;
using BankingApi.Shared.Contracts;

namespace BankingApi.Query.Accounts;

public class GetAllAccountsQueryHandler(IAccountRepository accountRepository)
    : IRequestHandler<GetAllAccountsQuery, IEnumerable<AccountResponse>>
{
    public async Task<IEnumerable<AccountResponse>> Handle(GetAllAccountsQuery request, CancellationToken cancellationToken = default)
    {
        var accounts = await accountRepository.GetAllAsync(cancellationToken);

        return accounts.Select(account => new AccountResponse
        {
            Id = account.Id,
            CustomerId = account.CustomerId,
            AccountNumber = account.AccountNumber,
            AccountType = account.AccountType,
            Balance = account.Balance,
            CreatedAt = account.CreatedAt
        });
    }
}
