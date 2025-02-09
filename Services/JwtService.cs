using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DotNetEnv; // Corrected namespace
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

public class JwtService
{
    private readonly string _secret;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expirationInMinutes;

    public JwtService()
    {
        Env.Load(); // Corrected method to load environment variables from .env file

        _secret =
            Environment.GetEnvironmentVariable("JWT_SECRET")
            ?? throw new ArgumentNullException("JWT Secret is missing.");
        _issuer =
            Environment.GetEnvironmentVariable("JWT_ISSUER")
            ?? throw new ArgumentNullException("JWT Issuer is missing.");
        _audience =
            Environment.GetEnvironmentVariable("JWT_AUDIENCE")
            ?? throw new ArgumentNullException("JWT Audience is missing.");

        if (
            !int.TryParse(
                Environment.GetEnvironmentVariable("JWT_EXPIRATION_IN_MINUTES"),
                out _expirationInMinutes
            )
        )
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
