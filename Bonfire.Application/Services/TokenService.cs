using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Bonfire.Application.Interfaces;
using Bonfire.Domain.Dtos.Responses;
using Bonfire.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Bonfire.Application.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    private readonly TimeProvider _timeProvider;

    public TokenService(IConfiguration configuration, TimeProvider timeProvider)
    {
        _configuration = configuration;
        _timeProvider = timeProvider;
    }

    public TokenResponse GenerateToken(User user)
    {
        var claims = new List<Claim>();

        claims.Add(new(ClaimTypes.Sid, user.Id.ToString()));

        var issuer = _configuration.GetValue<string>("AuthOptions:Issuer");
        var audience = _configuration.GetValue<string>("AuthOptions:Audience");
        var accessTokenValidityInDays = _configuration.GetValue<double>("AuthOptions:AccessTokenValidityInDays");

        var accessTokenValidityDateTime = _timeProvider.GetUtcNow()
            .AddDays(accessTokenValidityInDays)
            .DateTime;

        var key = _configuration.GetValue<string>("AuthOptions:Key");
        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!));

        var jwt = new JwtSecurityToken(issuer,
            audience,
            claims,
            expires: accessTokenValidityDateTime,
            signingCredentials: new(symmetricSecurityKey, SecurityAlgorithms.HmacSha256));

        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        return new(encodedJwt, accessTokenValidityDateTime);
    }
}