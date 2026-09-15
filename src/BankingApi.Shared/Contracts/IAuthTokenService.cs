namespace BankingApi.Shared.Contracts;

public interface IAuthTokenService
{
    (string Token, DateTime ExpiresAt) CreateToken(int userId, int customerId, string email, string role);
}