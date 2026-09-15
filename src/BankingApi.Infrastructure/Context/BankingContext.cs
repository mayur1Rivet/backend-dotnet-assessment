using BankingApi.Infrastructure.Entity;
using Microsoft.EntityFrameworkCore;

namespace BankingApi.Infrastructure.Context;

public class BankingContext(DbContextOptions<BankingContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Account> Accounts => Set<Account>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>()
            .HasIndex(c => c.Email)
            .IsUnique();

        modelBuilder.Entity<Account>()
            .HasIndex(a => a.AccountNumber)
            .IsUnique();

        modelBuilder.Entity<Account>()
            .HasOne(a => a.Customer)
            .WithMany()
            .HasForeignKey(a => a.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Customer>()
            .Property(c => c.FirstName)
            .HasMaxLength(100);

        modelBuilder.Entity<Customer>()
            .Property(c => c.LastName)
            .HasMaxLength(100);

        modelBuilder.Entity<Customer>()
            .Property(c => c.Email)
            .HasMaxLength(200);

        modelBuilder.Entity<Customer>()
            .Property(c => c.PhoneNumber)
            .HasMaxLength(50);
    }
}
