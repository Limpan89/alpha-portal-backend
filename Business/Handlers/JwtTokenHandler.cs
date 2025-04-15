using Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Business.Handlers;

public class JwtTokenHandler
{
    private readonly IConfiguration _configuration;

    public JwtTokenHandler(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(UserModel user, string? role = null)
    {
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!);
        var issuer = _configuration["Jwt:Issuer"];

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            //new(ClaimTypes.Email, user.Email)
        };

        if (role != null)
            claims.Add(new(ClaimTypes.Role, role));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = issuer,
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
