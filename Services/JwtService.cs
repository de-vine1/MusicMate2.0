using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

public class JwtService
{
    private readonly string _secret;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expirationInMinutes;

    public JwtService(IConfiguration config)
    {
        _secret =
            config["JwtSettings:Secret"]
            ?? throw new ArgumentNullException("JWT Secret is missing.");
        _issuer =
            config["JwtSettings:Issuer"]
            ?? throw new ArgumentNullException("JWT Issuer is missing.");
        _audience =
            config["JwtSettings:Audience"]
            ?? throw new ArgumentNullException("JWT Audience is missing.");

        if (!int.TryParse(config["JwtSettings:ExpirationInMinutes"], out _expirationInMinutes))
        {
            throw new ArgumentException("JWT ExpirationInMinutes is invalid.");
        }
    }

    public string GenerateToken(Guid userId, string email)
{
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var claims = new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()), // Now correctly stores Guid as string
        new Claim(JwtRegisteredClaimNames.Email, email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    };

    var token = new JwtSecurityToken(
        issuer: _issuer,
        audience: _audience,
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(_expirationInMinutes),
        signingCredentials: creds
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}
}