using Bonfire.Abstractions;
using Bonfire.Core.Dtos.Requests;
using Bonfire.Core.Entities;
using Bonfire.Core.Exceptions;
using Bonfire.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Bonfire.Application.Services;

public class LoginService(ITokenService tokenService, AppDbContext appDbContext) : ILoginService
{
    public async Task<string> Login(LoginRequest loginRequest)
    {
        var authorizedUser = await VerifyLoginCredentials(loginRequest.NickName, loginRequest.Password);
        if (authorizedUser is null)
        {
            throw new InvalidLoginCredentialsException();
        }
        
        var token = tokenService.GenerateToken(authorizedUser);
        return token;
    }

    public async Task<User?> VerifyLoginCredentials(string nickName, string password)
    {
        var user = await appDbContext.Users.AsNoTracking().FirstOrDefaultAsync(x =>
            x.Nickname == nickName);
        if (user is null)
        {
            throw new InvalidLoginCredentialsException();
        }
        var passwordMatch = BCrypt.Net.BCrypt.EnhancedVerify(password, user?.PasswordHash);
        return passwordMatch ? user : null;
    }
}
