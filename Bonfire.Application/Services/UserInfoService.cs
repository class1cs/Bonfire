using Bonfire.Abstractions;
using Bonfire.Core.Dtos.Response;
using Bonfire.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Bonfire.Application.Services;

public class UserInfoService(IUserService userService, AppDbContext appDbContext) : IUserInfoService
{
    public async Task<UserResponse> GetCurrentUserInfo()
    {
        var user = await userService.GetCurrentUser();
        return new UserResponse
        {
            Id = user.Id,
            NickName = user.Nickname
        };
    }

    public async Task<List<UserResponse>> SearchUser(string searchRequest)
    {
        var currentUser = await userService.GetCurrentUser();
        var users = await appDbContext.Users
            .Where(x => x.Nickname.Contains(searchRequest) && x.Nickname != currentUser.Nickname)
            .Select(x => new UserResponse { Id = x.Id, NickName = x.Nickname }).ToListAsync();
        return users;
    }
}