using Bonfire.Core.Dtos.Response;

namespace Bonfire.Abstractions;

public interface IUserInfoService
{
    Task<UserResponse> GetCurrentUserInfo();

    Task<List<UserResponse>> SearchUser(string searchRequest);
}