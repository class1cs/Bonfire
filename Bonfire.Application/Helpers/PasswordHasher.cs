using static BCrypt.Net.BCrypt;

namespace Bonfire.Application.Helpers;

public static class PasswordHasher
{
    public static string HashPassword(string password)
    {
        var passwordHash = EnhancedHashPassword(password);

        return passwordHash;
    }
}