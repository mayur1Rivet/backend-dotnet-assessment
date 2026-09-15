using BankingApi.DTO.Accounts;
using BankingApi.Infrastructure.IRepository;
using BankingApi.Shared.Contracts;

namespace BankingApi.Query.Accounts;

public class GetAccountsByCustomerIdQueryHandler(IAccountRepository accountRepository)
    : IRequestHandler<GetAccountsByCustomerIdQuery, IEnumerable<AccountResponse>>
{
    public async Task<IEnumerable<AccountResponse>> Handle(GetAccountsByCustomerIdQuery request, CancellationToken cancellationToken = default)
    {
        var accounts = await accountRepository.GetByCustomerIdAsync(request.CustomerId, cancellationToken);

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
