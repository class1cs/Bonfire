using static BCrypt.Net.BCrypt;

namespace Bonfire.Application.Services;

public class PasswordHasherService : IPasswordHasherService
{
    public string HashPassword(string password)
    {
        var passwordHash = EnhancedHashPassword(password);
        return passwordHash;
    }
}