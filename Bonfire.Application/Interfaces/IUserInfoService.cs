using Bonfire.Domain.Dtos.Responses;

namespace Bonfire.Application.Interfaces;

public interface IUserInfoService
{
    Task<UserResponse> GetCurrentUserInfo(CancellationToken cancellationToken);

    Task<UserResponse[]> SearchUser(string searchRequest, CancellationToken cancellationToken);
}