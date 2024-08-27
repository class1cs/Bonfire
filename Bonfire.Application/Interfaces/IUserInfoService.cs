using Bonfire.Core.Dtos.Response;

namespace Bonfire.Application.Interfaces;

public interface IUserInfoService
{
    Task<UserResponse> GetCurrentUserInfo();

    Task<UserResponse[]> SearchUser(string searchRequest);
}