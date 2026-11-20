using System.Threading.Tasks;
using baroc_intens.Data;
using baroc_intens.Models;
using Microsoft.EntityFrameworkCore;

namespace baroc_intens.Services;

public sealed class AuthService
{
    public async Task<AppUser?> AuthenticateAsync(string username, string password)
    {
        var normalizedUsername = username.Trim().ToLowerInvariant();
        if (normalizedUsername.Length == 0 || password.Length == 0)
        {
            return null;
        }

        await using var database = new BarrocDbContext();
        var user = await database.Users.FirstOrDefaultAsync(
            candidate => candidate.Username.ToLower() == normalizedUsername);

        return user is not null && PasswordHasher.Verify(password, user.PasswordSalt, user.PasswordHash)
            ? user
            : null;
    }
}
