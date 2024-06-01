using Bonfire.Core.Entities;

namespace Bonfire.Abstractions;

public interface ITokenService
{
    public Task<string> GenerateToken(User user);
}