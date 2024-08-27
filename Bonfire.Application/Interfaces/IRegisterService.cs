using Bonfire.Core.Dtos.Requests;

namespace Bonfire.Application.Interfaces;

public interface IRegisterService
{
    public Task<string> Register(RegisterRequest registerUser);

    public Task<bool> CheckUserExists(string login);
}