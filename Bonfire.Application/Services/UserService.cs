﻿using System.Security.Claims;
using Bonfire.Application.Interfaces;
using Bonfire.Domain.Entities;
using Bonfire.Persistance;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Bonfire.Application.Services;

public class UserService(IHttpContextAccessor httpContextAccessor, AppDbContext appDbContext) : IUserService
{
    public async Task<User> GetCurrentUser(CancellationToken cancellationToken)
    {
        var currentUserIdString =
            httpContextAccessor.HttpContext!.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)!.Value;

        var currentUserIdLong = Convert.ToInt64(currentUserIdString);

        var currentUser = await appDbContext.Users.Include(x => x.Conversations)
            .FirstAsync(x => x.Id == currentUserIdLong, cancellationToken);

        return currentUser;
    }
}