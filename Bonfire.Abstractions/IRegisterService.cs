using Bonfire.Core.Dtos.Requests;

namespace Bonfire.Abstractions;

public interface IRegisterService
{
    public Task<string> RegisterAsync(RegisterRequestDto registerUserDto);

    public Task<bool> CheckUserExists(string login, string passwordHash);
}