using Bonfire.Core.Dtos.Response;
using Bonfire.Core.Entities;

namespace Bonfire.Abstractions;

public interface IUserInfoService
{
    Task<UserResponse> GetCurrentUserInfo();
    
    Task<List<UserResponse>> SearchUser(string searchRequest);
}