using Bonfire.Abstractions;
using Bonfire.Core.Dtos.Requests;
using Bonfire.Core.Entities;
using Bonfire.Core.Exceptions;
using Bonfire.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Bonfire.Application.Services;

public class LoginService(ITokenService tokenService, AppDbContext appDbContext, IPasswordHasherService passwordHasherService) : ILoginService
{
    public async Task<string> Login(LoginRequestDto loginRequestDto)
    {
        var authorizedUser = await VerifyLoginCredentials(loginRequestDto.NickName, loginRequestDto.Password);
        if (authorizedUser is null)
        {
            throw new InvalidLoginCredentialsException();
        }

        return await tokenService.GenerateToken(authorizedUser);
    }

    public async Task<User?> VerifyLoginCredentials(string nickName, string password)
    {
        var user = await appDbContext.Users.AsNoTracking().FirstOrDefaultAsync(x =>
            x.NickName == nickName);
        if (user is null)
        {
            throw new InvalidLoginCredentialsException();
        }
        var passwordMatch = BCrypt.Net.BCrypt.EnhancedVerify(password, user?.PasswordHash);
        return passwordMatch ? user : null;
    }
}
