using Bonfire.Application.Interfaces;
using Bonfire.Domain.Dtos.Responses;
using Bonfire.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Bonfire.Application.Services;

public class UserInfoService(IUserService userService, AppDbContext appDbContext) : IUserInfoService
{
    public async Task<UserDto> GetCurrentUserInfo(CancellationToken cancellationToken)
    {
        var user = await userService.GetCurrentUser(cancellationToken);

        return new(user.Id, user.Nickname);
    }

    public async Task<UserDto[]> SearchUser(string userNickNameRequest, CancellationToken cancellationToken)
    {
        var currentUser = await userService.GetCurrentUser(cancellationToken);

        var users = await appDbContext.Users.AsNoTracking()
            .Where(x => x.Nickname.Contains(userNickNameRequest) && x.Nickname != currentUser.Nickname)
            .Select(x => new UserDto(x.Id, x.Nickname))
            .ToArrayAsync(cancellationToken);

        return users;
    }
}