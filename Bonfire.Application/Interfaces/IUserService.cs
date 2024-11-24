using Bonfire.Domain.Entities;

namespace Bonfire.Application.Interfaces;

public interface IUserService
{
    Task<User> GetCurrentUser(CancellationToken cancellationToken);
}