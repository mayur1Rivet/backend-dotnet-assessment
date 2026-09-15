using System.ComponentModel.DataAnnotations;

namespace BankingApi.Infrastructure.Entity;

public class User
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [MaxLength(30)]
    public string Role { get; set; } = "Customer";

    public int CustomerId { get; set; }

    public Customer Customer { get; set; } = null!;
}