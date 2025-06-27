using Bonfire.Domain.Dtos.Requests;
using Bonfire.Domain.Dtos.Responses;

namespace Bonfire.Application.Interfaces;

public interface IFriendRequestsService
{
    Task<FriendRequestResponse> SendFriendRequest(FriendRequestRequest friendRequestRequest, CancellationToken cancellationToken);
    
    Task<FriendRequestResponse> AcceptFriendRequest(long friendRequestId, CancellationToken cancellationToken);
    
    Task<FriendRequestResponse> DeclineFriendRequest(long friendRequestId, CancellationToken cancellationToken);
}