using Bonfire.Application.Interfaces;
using Bonfire.Domain.Dtos.Responses;
using Bonfire.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Bonfire.Application.Services;

public class UserInfoService(IUserService userService, AppDbContext appDbContext) : IUserInfoService
{
    public async Task<UserDto> GetCurrentUserInfo()
    {
        var user = await userService.GetCurrentUser();
        return new UserDto(user.Id, user.Nickname);
    }

    public async Task<UserDto[]> SearchUser(string searchRequest)
    {
        var currentUser = await userService.GetCurrentUser();
        var users = await appDbContext.Users
            .Where(x => x.Nickname.Contains(searchRequest) && x.Nickname != currentUser.Nickname)
            .Select(x => new UserDto(x.Id, x.Nickname)).ToArrayAsync();
        return users;
    }
}