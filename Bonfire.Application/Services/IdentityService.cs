using Bonfire.Application.Helpers;
using Bonfire.Application.Interfaces;
using Bonfire.Domain.Dtos.Requests;
using Bonfire.Domain.Dtos.Responses;
using Bonfire.Domain.Entities;
using Bonfire.Domain.Exceptions;
using Bonfire.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Bonfire.Application.Services;

public class IdentityService(ITokenService tokenService, AppDbContext appDbContext) : IIdentityService
{
    public async Task<TokenDto> Login(LoginRequestDto loginRequestDto, CancellationToken cancellationToken)
    {
        var authorizedUser = await VerifyLoginCredentials(loginRequestDto.NickName, loginRequestDto.Password, cancellationToken);

        if (authorizedUser is null)
        {
            throw new InvalidLoginCredentialsException();
        }

        var tokenDto = tokenService.GenerateToken(authorizedUser);

        return tokenDto;
    }

    public async Task<TokenDto> Register(RegisterRequestDto registerUser, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(registerUser.NickName) || string.IsNullOrWhiteSpace(registerUser.Password))
        {
            throw new InvalidRegistrationDataException();
        }

        if (await CheckUserExists(registerUser.NickName, cancellationToken))
        {
            throw new NicknameAlreadyExistsException();
        }

        var passwordHash = PasswordHasher.HashPassword(registerUser.Password);
        var userToAdd = new User(registerUser.NickName, passwordHash, new List<Conversation>());

        await appDbContext.Users.AddAsync(userToAdd, cancellationToken);
        await appDbContext.SaveChangesAsync(cancellationToken);
        var tokenDto = tokenService.GenerateToken(userToAdd);

        return tokenDto;
    }

    public async Task<bool> CheckUserExists(string login, CancellationToken cancellationToken)
    {
        var isUserExists = await appDbContext.Users.AsNoTracking()
            .AnyAsync(x => x.Nickname == login, cancellationToken);

        return isUserExists;
    }

    public async Task<User?> VerifyLoginCredentials(
        string nickName,
        string password,
        CancellationToken cancellationToken)
    {
        var user = await appDbContext.Users.AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.Nickname == nickName, cancellationToken);

        if (user is null)
        {
            throw new InvalidLoginCredentialsException();
        }

        var passwordMatch = BCrypt.Net.BCrypt.EnhancedVerify(password, user?.PasswordHash);

        return passwordMatch
            ? user
            : null;
    }
}