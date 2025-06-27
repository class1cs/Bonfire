using Bonfire.Domain.Dtos.Responses;
using Bonfire.Domain.Entities;

namespace Bonfire.Application.Interfaces;

public interface ITokenService
{
    public TokenResponse GenerateToken(User user);
}