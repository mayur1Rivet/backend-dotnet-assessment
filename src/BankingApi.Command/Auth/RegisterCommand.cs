using BankingApi.DTO.Auth;
using BankingApi.Shared.Contracts;

namespace BankingApi.Command.Auth;

public record RegisterCommand(RegisterRequest User) : IRequest<AuthResponse>;