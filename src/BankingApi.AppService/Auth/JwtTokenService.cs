using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BankingApi.Shared.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BankingApi.AppService.Auth;

public sealed class JwtTokenService(IConfiguration configuration) : IAuthTokenService
{
    public (string Token, DateTime ExpiresAt) CreateToken(int userId, int customerId, string email, string role)
    {
        var jwt = configuration.GetSection("Jwt");
        var key = jwt["Key"] ?? throw new InvalidOperationException("JWT signing key is not configured.");
        var issuer = jwt["Issuer"] ?? throw new InvalidOperationException("JWT issuer is not configured.");
        var audience = jwt["Audience"] ?? throw new InvalidOperationException("JWT audience is not configured.");
        var expiresAt = DateTime.UtcNow.AddMinutes(jwt.GetValue<int>("ExpiryMinutes", 60));
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim("customer_id", customerId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(ClaimTypes.Role, role)
        };
        var credentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(issuer, audience, claims, expires: expiresAt, signingCredentials: credentials);
        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}