using Bonfire.Core.Entities;

namespace Bonfire.Abstractions;

public interface IUserService
{
    Task<User> GetCurrentUser();
}