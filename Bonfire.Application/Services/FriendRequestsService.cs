using Bonfire.Application.Interfaces;
using Bonfire.Domain.Dtos.Requests;
using Bonfire.Domain.Dtos.Responses;
using Bonfire.Domain.Exceptions;
using Bonfire.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Bonfire.Application.Services;

public class FriendRequestsService(IUserService userService, AppDbContext appDbContext) : IFriendRequestsService
{
    public async Task<FriendRequestResponse> SendFriendRequest(FriendRequestRequest friendRequestRequest, CancellationToken cancellationToken)
    {
        var currentUser = await userService.GetCurrentUser(cancellationToken);
        var friendUser = await appDbContext.Users.Include(x => x.FriendRequests)
            .Include(user => user.Friends)
            .FirstOrDefaultAsync(x => x.Id == friendRequestRequest.UserId, cancellationToken: cancellationToken);

        if (friendUser == null)
        {
            throw new UserNotFoundException();
        }

        if (friendUser.FriendRequests.Any(x => x.Sender.Id == currentUser.Id))
        {
            throw new FriendRequestAlreadySentException();
        }
        
        if (friendUser.Friends.Any(x => x.Id == currentUser.Id))
        {
            throw new AlreadyFriendException();
        }

        return null;
    }

    public Task<FriendRequestResponse> AcceptFriendRequest(long friendRequestId, CancellationToken cancellationToken)
    {
        return null;
    }

    public Task<FriendRequestResponse> DeclineFriendRequest(long friendRequestId, CancellationToken cancellationToken)
    {
        return null;
    }
    
    // TODO:
    // Доделать эту систему.
}