using Bonfire.Domain.Dtos.Responses;

namespace Bonfire.Application.Interfaces;

public interface IUserInfoService
{
    Task<UserDto> GetCurrentUserInfo(CancellationToken cancellationToken);

    Task<UserDto[]> SearchUser(string searchRequest, CancellationToken cancellationToken);
}