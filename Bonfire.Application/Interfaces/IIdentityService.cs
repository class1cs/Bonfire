using Bonfire.Domain.Dtos.Requests;
using Bonfire.Domain.Dtos.Responses;
using Bonfire.Domain.Entities;

namespace Bonfire.Application.Interfaces;

public interface IIdentityService
{
    Task<TokenResponse> Login(LoginRequest loginRequest, CancellationToken cancellationToken);

    Task<TokenResponse> Register(RegisterRequest registerUser, CancellationToken cancellationToken);

    Task<bool> CheckUserExists(string login, CancellationToken cancellationToken);

    Task<User?> VerifyLoginCredentials(
        string nickName,
        string password,
        CancellationToken cancellationToken);
}