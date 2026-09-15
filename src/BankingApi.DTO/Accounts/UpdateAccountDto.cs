namespace BankingApi.DTO.Accounts;

public class UpdateAccountDto
{
    public string AccountType { get; set; } = string.Empty;
    public decimal Balance { get; set; }
}
