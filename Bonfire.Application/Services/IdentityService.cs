using Bonfire.Application.Helpers;
using Bonfire.Application.Interfaces;
using Bonfire.Domain.Dtos.Requests;
using Bonfire.Domain.Entities;
using Bonfire.Domain.Exceptions;
using Bonfire.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Bonfire.Application.Services;



public class IdentityService(ITokenService tokenService, AppDbContext appDbContext) : IIdentityService
{
    public async Task<string> Login(LoginRequestDto loginRequestDto)
    {
        var authorizedUser = await VerifyLoginCredentials(loginRequestDto.NickName, loginRequestDto.Password);
        if (authorizedUser is null)
        {
            throw new InvalidLoginCredentialsException();
        }

        var token = tokenService.GenerateToken(authorizedUser);
        return token;
    }

    public async Task<string> Register(RegisterRequestDto registerUser)
    {
        if (string.IsNullOrWhiteSpace(registerUser.NickName) || string.IsNullOrWhiteSpace(registerUser.Password))
        {
            throw new InvalidRegistrationDataException();
        }
            

        if (await CheckUserExists(registerUser.NickName))
        {
            throw new NicknameAlreadyExistsException();
        }
          

        var passwordHash = PasswordHasher.HashPassword(registerUser.Password);
        var userToAdd = new User(registerUser.NickName, passwordHash, new List<Conversation>());

        await appDbContext.Users.AddAsync(userToAdd);
        await appDbContext.SaveChangesAsync();
        return tokenService.GenerateToken(userToAdd);
    }

    public async Task<bool> CheckUserExists(string login)
    {
        var isUserExists = await appDbContext.Users.AsNoTracking().AnyAsync(x => x.Nickname == login);
        return isUserExists;
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