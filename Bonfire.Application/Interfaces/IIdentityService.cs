using Bonfire.Domain.Dtos.Requests;
using Bonfire.Domain.Dtos.Responses;
using Bonfire.Domain.Entities;

namespace Bonfire.Application.Interfaces;

public interface IIdentityService
{
    Task<TokenDto> Login(LoginRequestDto loginRequestDto, CancellationToken cancellationToken);

    Task<TokenDto> Register(RegisterRequestDto registerUser, CancellationToken cancellationToken);

    Task<bool> CheckUserExists(string login, CancellationToken cancellationToken);

    Task<User?> VerifyLoginCredentials(
        string nickName,
        string password,
        CancellationToken cancellationToken);
}