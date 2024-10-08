using Bonfire.Domain.Dtos.Requests;
using Bonfire.Domain.Entities;

namespace Bonfire.Application.Interfaces;

public interface IIdentityService
{
    Task<string> Login(LoginRequestDto loginRequestDto, CancellationToken cancellationToken);
    
    Task<string> Register(RegisterRequestDto registerUser, CancellationToken cancellationToken);
    
    Task<bool> CheckUserExists(string login, CancellationToken cancellationToken);
    
    Task<User?> VerifyLoginCredentials(string nickName, string password, CancellationToken cancellationToken);
}