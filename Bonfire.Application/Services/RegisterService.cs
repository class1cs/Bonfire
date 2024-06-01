using System.Security.Authentication;
using Bonfire.Abstractions;
using Bonfire.Core.Dtos.Requests;
using Bonfire.Core.Entities;
using Bonfire.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Bonfire.Application.Services;

public class RegisterService(IPasswordHasherService passwordHasherService, AppDbContext appDbContext, ITokenService tokenService) : IRegisterService
{
    public async Task<string> RegisterAsync(RegisterRequestDto registerUserDto)
    {
        var passwordHash = passwordHasherService.HashPassword(registerUserDto.Password);
        var userToAdd = new User(registerUserDto.NickName, passwordHash, new List<DirectChat>());

        if (await CheckUserExists(registerUserDto.NickName, passwordHash))
        {
            throw new InvalidCredentialException("Этот аккаунт уже существует!");
        }
        await appDbContext.Users.AddAsync(userToAdd);
        await appDbContext.SaveChangesAsync();
        return await tokenService.GenerateToken(userToAdd);
    }

    public async Task<bool> CheckUserExists(string login, string passwordHash)
    {
        var isUserExists = await appDbContext.Users.AsNoTracking().AnyAsync(x => x.NickName == login && x.PasswordHash == passwordHash);
        return isUserExists;
    }
}