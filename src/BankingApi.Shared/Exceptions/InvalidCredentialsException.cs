namespace BankingApi.Shared.Exceptions;

public sealed class InvalidCredentialsException()
    : Exception("The email or password is incorrect.");