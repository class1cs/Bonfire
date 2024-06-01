﻿using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Bonfire.Abstractions;
using Bonfire.Core.Entities;
using Bonfire.Persistance;
using Microsoft.IdentityModel.Tokens;

namespace Bonfire.Application.Services;

public class TokenService(AppDbContext appDbContext) : ITokenService
{

    
    public async Task<string> GenerateToken(User user)
    {
        var claims = new List<Claim>();
        claims.Add(new("Id", user.Id.ToString()));

        var jwt = new JwtSecurityToken(AuthOptions.Issuer,
            AuthOptions.Audience,
            claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromDays(2)),
            signingCredentials: new(AuthOptions.GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256));

        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        return encodedJwt;
    }
}