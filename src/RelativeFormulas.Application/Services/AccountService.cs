using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using RelativeFormulas.Domain.Entities;
using RelativeFormulas.Infrastructure.Data;

namespace RelativeFormulas.Application.Services;

public class AccountService
{
    private readonly AppDbContext _context;

    public AccountService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> RegisterAsync(string email, string displayName, string password)
    {
        if (await _context.Users.AnyAsync(u => u.Email == email))
            return false;

        var user = new User
        {
            Email = email,
            DisplayName = displayName,
            PasswordHash = HashPassword(password),
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<User?> ValidateCredentialsAsync(string email, string password)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        if (user is null)
            return null;

        return VerifyPassword(password, user.PasswordHash) ? user : null;
    }

    private static string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, 32);
        return Convert.ToBase64String(salt) + "." + Convert.ToBase64String(hash);
    }

    private static bool VerifyPassword(string password, string storedHash)
    {
        var parts = storedHash.Split('.');
        if (parts.Length != 2) return false;
        var salt = Convert.FromBase64String(parts[0]);
        var expectedHash = Convert.FromBase64String(parts[1]);
        var actualHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, 32);
        return CryptographicOperations.FixedTimeEquals(expectedHash, actualHash);
    }
}
