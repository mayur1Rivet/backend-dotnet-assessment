using BankingApi.Infrastructure.Entity;

namespace BankingApi.Infrastructure.IRepository;

public interface ICustomerRepository
{
    Task<List<Customer>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Customer?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Customer> AddAsync(Customer customer, CancellationToken cancellationToken = default);
    Task<Customer> UpdateAsync(Customer customer, CancellationToken cancellationToken = default);
    Task DeleteAsync(Customer customer, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
