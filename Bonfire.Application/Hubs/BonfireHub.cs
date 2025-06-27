using Bonfire.Application.Interfaces;
using Bonfire.Domain.Dtos.Requests;
using Bonfire.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Bonfire.Application.Hubs;

[Authorize]
public class BonfireHub : Hub<IBonfireHubClient>
{
    
    private readonly IUserService _userService;

  
    
    public BonfireHub(IUserService userService)
    {
        _userService = userService;
    }
    
    public override async Task OnConnectedAsync()
    {
        var user =  await _userService.GetCurrentUser(default);
        var userConversations = user.Conversations;
        foreach(var group in userConversations)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"conv_{group.Id}");
        }
        await base.OnConnectedAsync();
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var user =  await _userService.GetCurrentUser(default);
        var userConversations = user.Conversations;
        foreach(var group in userConversations)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"conv_{group.Id}");
        }
    }
}
