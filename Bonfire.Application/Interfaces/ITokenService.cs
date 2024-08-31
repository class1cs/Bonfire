using Bonfire.Domain.Entities;

namespace Bonfire.Application.Interfaces;

public interface ITokenService
{
    public string GenerateToken(User user);
}