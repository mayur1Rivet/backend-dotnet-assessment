using BankingApi.DTO.Auth;
using BankingApi.Infrastructure.Entity;
using BankingApi.Infrastructure.IRepository;
using BankingApi.Shared.Contracts;
using BankingApi.Shared.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace BankingApi.Command.Auth;

public sealed class LoginCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher<User> passwordHasher,
    IAuthTokenService tokenService)
    : IRequestHandler<LoginCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByEmailAsync(request.User.Email.Trim(), cancellationToken);
        if (user is null || passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.User.Password) == PasswordVerificationResult.Failed)
        {
            throw new InvalidCredentialsException();
        }

        var (token, expiresAt) = tokenService.CreateToken(user.Id, user.CustomerId, user.Email, user.Role);
        return new AuthResponse(token, expiresAt, user.Id, user.CustomerId, user.Role);
    }
}