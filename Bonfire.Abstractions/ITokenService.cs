using Bonfire.Core.Entities;

namespace Bonfire.Abstractions;

public interface ITokenService
{
    public string GenerateToken(User user);
}