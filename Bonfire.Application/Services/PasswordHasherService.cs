using Bonfire.Core.Dtos.Requests;
using static BCrypt.Net.BCrypt;

namespace Bonfire.Application.Services;


public class PasswordHasherService : IPasswordHasherService
{
    public string HashPassword(string password)
    {
        var passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(password);
        return passwordHash;
    }
    
 
}

