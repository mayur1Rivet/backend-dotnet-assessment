using BankingApi.Infrastructure.Context;
using BankingApi.Infrastructure.Entity;
using BankingApi.Infrastructure.IRepository;
using Microsoft.EntityFrameworkCore;

namespace BankingApi.Infrastructure.Repository;

public class CustomerRepository(BankingContext context) : ICustomerRepository
{
    public async Task<List<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Customers
            .AsNoTracking()
            .OrderBy(c => c.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<Customer?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await context.Customers
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await context.Customers
            .AnyAsync(c => c.Email.ToLower() == email.ToLower(), cancellationToken);
    }

    public async Task<Customer> AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        await context.Customers.AddAsync(customer, cancellationToken);
        return customer;
    }

    public async Task<Customer> UpdateAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        context.Customers.Update(customer);
        await Task.CompletedTask;
        return customer;
    }

    public async Task DeleteAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        context.Customers.Remove(customer);
        await Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }
}
