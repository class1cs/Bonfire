using Bonfire.Abstractions;
using Bonfire.Core.Dtos.Requests;
using Bonfire.Core.Entities;
using Bonfire.Core.Exceptions;
using Bonfire.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Bonfire.Application.Services;

public class RegisterService(
    IPasswordHasherService passwordHasherService,
    AppDbContext appDbContext,
    ITokenService tokenService) : IRegisterService
{
    public async Task<string> Register(RegisterRequest registerUser)
    {
        if (string.IsNullOrWhiteSpace(registerUser.NickName) || string.IsNullOrWhiteSpace(registerUser.Password))
            throw new InvalidRegistrationDataException();

        if (await CheckUserExists(registerUser.NickName)) throw new NicknameAlreadyExistsException();

        var passwordHash = passwordHasherService.HashPassword(registerUser.Password);
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
}