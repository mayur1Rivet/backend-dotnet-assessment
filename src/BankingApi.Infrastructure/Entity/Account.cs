using System.ComponentModel.DataAnnotations;

namespace BankingApi.Infrastructure.Entity;

public class Account
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    [Required]
    [MaxLength(50)]
    public string AccountNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string AccountType { get; set; } = string.Empty;

    public decimal Balance { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Customer Customer { get; set; } = null!;
}
