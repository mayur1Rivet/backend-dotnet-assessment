namespace BankingApi.DTO.Accounts;

public class CreateAccountDto
{
    public int CustomerId { get; set; }
    public string AccountType { get; set; } = "Savings";
    public decimal Balance { get; set; }
}
