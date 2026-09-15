using BankingApi.Infrastructure.Context;
using BankingApi.Infrastructure.Entity;
using BankingApi.Infrastructure.IRepository;
using Microsoft.EntityFrameworkCore;

namespace BankingApi.Infrastructure.Repository;

public class AccountRepository(BankingContext context) : IAccountRepository
{
    public async Task<List<Account>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Accounts
            .AsNoTracking()
            .OrderBy(a => a.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<Account?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await context.Accounts
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<List<Account>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default)
    {
        return await context.Accounts
            .AsNoTracking()
            .Where(a => a.CustomerId == customerId)
            .OrderBy(a => a.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<Account> AddAsync(Account account, CancellationToken cancellationToken = default)
    {
        await context.Accounts.AddAsync(account, cancellationToken);
        return account;
    }

    public async Task<Account> UpdateAsync(Account account, CancellationToken cancellationToken = default)
    {
        context.Accounts.Update(account);
        await Task.CompletedTask;
        return account;
    }

    public async Task DeleteAsync(Account account, CancellationToken cancellationToken = default)
    {
        context.Accounts.Remove(account);
        await Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }
}
