using BankingApi.Infrastructure.Context;
using BankingApi.Infrastructure.Entity;
using BankingApi.Infrastructure.IRepository;
using Microsoft.EntityFrameworkCore;

namespace BankingApi.Infrastructure.Repository;

public class UserRepository(BankingContext context) : IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await context.Users
            .Include(user => user.Customer)
            .FirstOrDefaultAsync(user => user.Email.ToLower() == email.ToLower(), cancellationToken);
    }

    public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await context.Users.AddAsync(user, cancellationToken);
        return user;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }
}