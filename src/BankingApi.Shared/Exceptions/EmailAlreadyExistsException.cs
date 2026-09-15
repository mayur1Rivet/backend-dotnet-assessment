namespace BankingApi.Shared.Exceptions;

public sealed class EmailAlreadyExistsException(string email)
    : Exception($"A customer with email '{email}' already exists.");