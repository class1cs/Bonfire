﻿using Bonfire.Application.Interfaces;
using Bonfire.Domain.Dtos.Requests;
using Bonfire.Domain.Dtos.Responses;
using Bonfire.Domain.Entities;
using Bonfire.Domain.Exceptions;
using Bonfire.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Bonfire.Application.Services;

public class ConversationsService(AppDbContext dbContext, IUserService userService) : IConversationsService
{
    public async Task<ConversationResponse> CreateConversation(
        ConversationRequest conversationRequest,
        CancellationToken cancellationToken)
    {
        var currentUser = await userService.GetCurrentUser(cancellationToken);

        var receivers = await dbContext.Users
            .Where(u => conversationRequest.UsersIds.Contains(u.Id))
            .ToListAsync(cancellationToken);

        if (receivers.Count != conversationRequest.UsersIds.Count)
        {
            throw new WrongConversationParticipantsIdsException();
        }

        if (receivers.Contains(currentUser))
        {
            throw new ReceiverEqualsSenderException();
        }

        receivers.Add(currentUser);

        var participants = new List<User>(receivers);

        var conversationType = receivers.Count > 2 ? ConversationType.Conversation : ConversationType.Dialogue;

        var existingChat = await dbContext.Conversations
            .Include(x => x.Participants)
            .FirstOrDefaultAsync(p => p.Participants
                .All(c => receivers.Contains(c) && p.Type == ConversationType.Dialogue), cancellationToken);

        if (existingChat is not null)
        {
            return new ConversationResponse(
                existingChat.Id,
                existingChat.Participants.Select(x => new UserResponse(x.Id, x.Nickname)).ToArray(), 
                existingChat.Type);
        }

        var conversation = new Conversation(new List<Message>(), participants, conversationType);

        await dbContext.Conversations.AddAsync(conversation, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new(conversation.Id,
            conversation.Participants.Select(x => new UserResponse(x.Id, x.Nickname))
                .ToArray(), conversation.Type);
    }

    public async Task<ConversationResponse[]> GetConversations(
        CancellationToken cancellationToken,
        long offsetMessageId = 0,
        short limit = 50)
    {
        var currentUser = await userService.GetCurrentUser(cancellationToken);

        var conversations = await dbContext.Conversations
            .AsNoTracking()
            .Include(x => x.Participants)
            .Where(x => x.Participants.Any(x => x.Id == currentUser.Id))
            .Where(b => b.Id > offsetMessageId)
            .OrderByDescending(x => x.Id)
            .Take(limit)
            .ToArrayAsync(cancellationToken);

        var conversationResponses = conversations.Select(x => new ConversationResponse(x.Id, x.Participants.Select(user => new UserResponse(user.Id, user.Nickname)).ToArray(), x.Type)).ToArray();

        return conversationResponses;
    }
}