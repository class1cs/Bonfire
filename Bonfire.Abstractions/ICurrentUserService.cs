using Bonfire.Core.Entities;

namespace Bonfire.Abstractions;

public interface ICurrentUserService
{
    
    Task<User> GetCurrentUser();
}