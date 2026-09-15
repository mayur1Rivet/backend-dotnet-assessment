using BankingApi.DTO.Auth;
using BankingApi.Infrastructure.Entity;
using BankingApi.Infrastructure.IRepository;
using BankingApi.Shared.Contracts;
using BankingApi.Shared.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace BankingApi.Command.Auth;

public sealed class RegisterCommandHandler(
    ICustomerRepository customerRepository,
    IUserRepository userRepository,
    IPasswordHasher<User> passwordHasher,
    IAuthTokenService tokenService)
    : IRequestHandler<RegisterCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken = default)
    {
        var email = request.User.Email.Trim();
        if (await userRepository.GetByEmailAsync(email, cancellationToken) is not null ||
            await customerRepository.ExistsByEmailAsync(email, cancellationToken))
        {
            throw new EmailAlreadyExistsException(email);
        }

        var customer = new Customer
        {
            FirstName = request.User.FirstName.Trim(),
            LastName = request.User.LastName.Trim(),
            Email = email,
            PhoneNumber = string.IsNullOrWhiteSpace(request.User.PhoneNumber) ? null : request.User.PhoneNumber.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        await customerRepository.AddAsync(customer, cancellationToken);
        var user = new User
        {
            Email = email,
            Role = "Customer",
            Customer = customer
        };
        user.PasswordHash = passwordHasher.HashPassword(user, request.User.Password);
        await userRepository.AddAsync(user, cancellationToken);
        await userRepository.SaveChangesAsync(cancellationToken);

        var (token, expiresAt) = tokenService.CreateToken(user.Id, customer.Id, user.Email, user.Role);
        return new AuthResponse(token, expiresAt, user.Id, customer.Id, user.Role);
    }
}