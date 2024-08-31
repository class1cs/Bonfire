using Bonfire.Domain.Dtos.Requests;
using Bonfire.Domain.Entities;

namespace Bonfire.Application.Interfaces;

public interface IIdentityService
{
    Task<string> Login(LoginRequestDto loginRequestDto);
    Task<string> Register(RegisterRequestDto registerUser);
    Task<bool> CheckUserExists(string login);
    Task<User?> VerifyLoginCredentials(string nickName, string password);
}