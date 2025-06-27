using Bonfire.Application.Interfaces;
using Bonfire.Domain.Dtos.Requests;
using Bonfire.Domain.Dtos.Responses;
using Bonfire.Domain.Entities;
using Microsoft.AspNetCore.SignalR;

namespace Bonfire.Application.Hubs;

public interface IBonfireHubClient
{
    Task ReceiveMessage(MessageResponse messageResponse);
}
