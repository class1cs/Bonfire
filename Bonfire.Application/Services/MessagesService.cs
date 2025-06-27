using Bonfire.Application.Hubs;
using Bonfire.Application.Interfaces;
using Bonfire.Domain.Dtos.Requests;
using Bonfire.Domain.Dtos.Responses;
using Bonfire.Domain.Entities;
using Bonfire.Domain.Exceptions;
using Bonfire.Persistance;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Bonfire.Application.Services;

public class MessagesService(
    AppDbContext dbContext,
    IUserService userService,
    TimeProvider timeProvider,
    IHubContext<BonfireHub, IBonfireHubClient> hubContext) : IMessagesService
{
    public async Task<MessageResponse> SendMessage(
        MessageRequest messageRequest,
        long conversationId,
        CancellationToken cancellationToken)
    {
        var currentUser = await userService.GetCurrentUser(cancellationToken);

        var conversation = await dbContext.Conversations.Include(x => x.Messages)
            .Include(conversation => conversation.Participants)
            .FirstOrDefaultAsync(x => x.Id == conversationId, cancellationToken);

        if (conversation == null)
        {
            throw new ConversationNotFoundException();
        }

        if (conversation.Participants.All(x => x.Id != currentUser.Id))
        {
            throw new AccessToConversationDeniedException();
        }

        var message = new Message(messageRequest.Text, currentUser, timeProvider.GetUtcNow());
        conversation.Messages.Add(message);
        await dbContext.SaveChangesAsync(cancellationToken);

        var messageResponse = new MessageResponse(message.Id, new UserResponse(message.Author.Id, message.Author.Nickname), message.Text,
            message.SentTime);
        await hubContext.Clients.Group($"conv_{conversation.Id}")
            .ReceiveMessage(messageResponse);
        return messageResponse;
    }

    public async Task<MessageResponse> EditMessage(
        MessageRequest messageRequest,
        long messageId,
        long conversationId,
        CancellationToken cancellationToken)
    {
        var currentUser = await userService.GetCurrentUser(cancellationToken);

        var conversation = await dbContext.Conversations.FirstOrDefaultAsync(x => x.Id == conversationId, cancellationToken);

        var message = await dbContext.Messages.Include(message => message.Author)
            .FirstOrDefaultAsync(x => x.Id == messageId && x.ConversationId == conversationId, cancellationToken);

        if (conversation is null)
        {
            throw new ConversationNotFoundException();
        }

        if (message is null)
        {
            throw new MessageNotFoundException();
        }

        if (message.Author.Id != currentUser?.Id)
        {
            throw new AccessToMessageDeniedException();
        }

        message.Text = messageRequest.Text;
        await dbContext.SaveChangesAsync(cancellationToken);

        return new(message.Id,
            new(message.Author.Id,
                message.Author.Nickname),
            message.Text,
            message.SentTime);
    }

    public async Task<MessageResponse> RemoveMessage(
        long messageId,
        long conversationId,
        CancellationToken cancellationToken)
    {
        var currentUser = await userService.GetCurrentUser(cancellationToken);

        var message = await dbContext.Messages
            .Include(message => message.Author)
            .FirstOrDefaultAsync(x => x.Id == messageId && x.ConversationId == conversationId, cancellationToken);

        if (message == null)
        {
            throw new MessageNotFoundException();
        }

        if (message.Author.Id != currentUser.Id)
        {
            throw new AccessToMessageDeniedException();
        }

        dbContext.Messages.Remove(message);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new(message.Id,
            new(message.Author.Id,
                message.Author.Nickname),
            message.Text,
            message.SentTime);
    }

    public async Task<MessagesResponse> GetMessages(
        CancellationToken cancellationToken,
        long conversationId,
        long offsetMessageId = 0,
        short limit = 50)
    {
        var currentUser = await userService.GetCurrentUser(cancellationToken);
        
        var conversation = await dbContext.Conversations
            .AsNoTracking()
            .Include(x => x.Participants)
            .FirstOrDefaultAsync(x => x.Id == conversationId, cancellationToken);

        if (conversation == null)
        {
            throw new ConversationNotFoundException();
        }
        
        if (conversation.Participants.All(x => x.Id != currentUser.Id))
        {
            throw new AccessToConversationDeniedException();
        }
        
        var messagesResponses = await dbContext.Messages
            .AsNoTracking()
            .Where(b => b.ConversationId == conversationId && b.Id > offsetMessageId)
            .OrderBy(b => b.Id)
            .Take(limit)
            .Select(x => new MessageResponse(x.Id, new UserResponse(x.Author.Id, x.Author.Nickname), x.Text, x.SentTime))
            .ToArrayAsync(cancellationToken);

        return new MessagesResponse(conversation.Id, messagesResponses);
    }
}