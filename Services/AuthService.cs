using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using MusicMateAPI.Data;
using MusicMateAPI.Models;

public interface IAuthService
{
    Task<string> RegisterUserAsync(string email, string password);
    Task<string> AuthenticateUserAsync(string email, string password);
}

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly JwtService _jwtService;

    public AuthService(AppDbContext context, JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    // Register a new user
    public async Task<string> RegisterUserAsync(string email, string password)
    {
        // Check if user already exists
        if (await _context.Users.AnyAsync(u => u.Email == email))
        {
            throw new ArgumentException("User with this email already exists.");
        }

        // Generate password salt
        var salt = GenerateSalt();
        var passwordHash = HashPassword(password, salt);

        // Create and save the user
        var user = new User
        {
            Id = Guid.NewGuid(), // Generate a new Guid for the user
            Email = email,
            PasswordHash = passwordHash,
            PasswordSalt = salt, // Store the salt as byte array
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Generate and return JWT Token
        return _jwtService.GenerateToken(user.Id, user.Email);
    }

    // Authenticate user and return a token
    public async Task<string> AuthenticateUserAsync(string email, string password)
    {
        // Find user by email
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            throw new ArgumentException("Invalid credentials.");
        }

        // Validate password
        var isPasswordValid = VerifyPassword(password, user.PasswordHash, user.PasswordSalt);
        if (!isPasswordValid)
        {
            throw new ArgumentException("Invalid credentials.");
        }

        // Generate and return JWT Token
        return _jwtService.GenerateToken(user.Id, user.Email);
    }

    // Hash password using PBKDF2
    private string HashPassword(string password, byte[] salt)
    {
        var hash = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 256 / 8
        );

        return Convert.ToBase64String(hash); // Convert the hash to base64 before storing
    }

    // Verify if the password is valid
    private bool VerifyPassword(string password, string storedHash, byte[] storedSalt)
    {
        var hash = HashPassword(password, storedSalt);
        return storedHash == hash;
    }

    // Generate a salt for password hashing
    private byte[] GenerateSalt()
    {
        var salt = new byte[128 / 8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }
        return salt;
    }
}
