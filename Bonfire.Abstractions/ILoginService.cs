using Bonfire.Core.Dtos.Requests;
using Bonfire.Core.Entities;

namespace Bonfire.Abstractions;

public interface ILoginService
{
    public Task<string> Login(LoginRequestDto loginRequestDto);

    public Task<User?> VerifyLoginCredentials(string nickName, string password);
}