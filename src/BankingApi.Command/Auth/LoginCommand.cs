using BankingApi.DTO.Auth;
using BankingApi.Shared.Contracts;

namespace BankingApi.Command.Auth;

public record LoginCommand(LoginRequest User) : IRequest<AuthResponse>;