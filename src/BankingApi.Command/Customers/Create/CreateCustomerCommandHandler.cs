using BankingApi.DTO.Customers;
using BankingApi.Infrastructure.Entity;
using BankingApi.Infrastructure.IRepository;
using BankingApi.Shared.Contracts;
using BankingApi.Shared.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace BankingApi.Command.Customers;

public class CreateCustomerCommandHandler(
    ICustomerRepository customerRepository,
    IUserRepository userRepository,
    IPasswordHasher<User> passwordHasher)
    : IRequestHandler<CreateCustomerCommand, CustomerResponse>
{
    public async Task<CustomerResponse> Handle(CreateCustomerCommand request, CancellationToken cancellationToken = default)
    {
        var email = request.Customer.Email.Trim();
        if (await customerRepository.ExistsByEmailAsync(email, cancellationToken) ||
            await userRepository.GetByEmailAsync(email, cancellationToken) is not null)
        {
            throw new EmailAlreadyExistsException(email);
        }

        var customer = new Customer
        {
            FirstName = request.Customer.FirstName.Trim(),
            LastName = request.Customer.LastName.Trim(),
            Email = email,
            PhoneNumber = string.IsNullOrWhiteSpace(request.Customer.PhoneNumber)
                ? null
                : request.Customer.PhoneNumber.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        await customerRepository.AddAsync(customer, cancellationToken);
        var user = new User
        {
            Email = email,
            Role = "Customer",
            Customer = customer
        };
        user.PasswordHash = passwordHasher.HashPassword(user, request.Customer.Password);
        await userRepository.AddAsync(user, cancellationToken);
        await userRepository.SaveChangesAsync(cancellationToken);

        return new CustomerResponse
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email,
            PhoneNumber = customer.PhoneNumber,
            CreatedAt = customer.CreatedAt
        };
    }
}
