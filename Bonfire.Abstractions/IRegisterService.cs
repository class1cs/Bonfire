using Bonfire.Core.Dtos.Requests;

namespace Bonfire.Abstractions;

public interface IRegisterService
{
    public Task<string> Register(RegisterRequest registerUser);

    public Task<bool> CheckUserExists(string login);
}