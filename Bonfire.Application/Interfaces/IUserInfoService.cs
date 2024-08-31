using Bonfire.Domain.Dtos.Responses;

namespace Bonfire.Application.Interfaces;

public interface IUserInfoService
{
    Task<UserDto> GetCurrentUserInfo();

    Task<UserDto[]> SearchUser(string searchRequest);
}