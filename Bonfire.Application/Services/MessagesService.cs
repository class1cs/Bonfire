﻿using Bonfire.Application.Interfaces;
using Bonfire.Domain.Dtos.Requests;
using Bonfire.Domain.Dtos.Responses;
using Bonfire.Domain.Entities;
using Bonfire.Domain.Exceptions;
using Bonfire.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Bonfire.Application.Services;

public class MessagesService(
    AppDbContext dbContext,
    IUserService userService,
    TimeProvider timeProvider) : IMessagesService
{
    public async Task<MessageDto> SendMessage(
        MessageRequestDto messageRequestDto,
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

        if (string.IsNullOrWhiteSpace(messageRequestDto.Text))
        {
            throw new EmptyMessageTextException();
        }

        var message = new Message(messageRequestDto.Text, currentUser, timeProvider.GetUtcNow());
        conversation.Messages.Add(message);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new(message.Id,
            new(message.Author.Id,
                message.Author.Nickname),
            message.Text,
            message.SentTime);
    }

    public async Task<MessageDto> EditMessage(
        MessageRequestDto messageRequestDto,
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

        if (string.IsNullOrWhiteSpace(messageRequestDto.Text))
        {
            throw new EmptyMessageTextException();
        }

        message.Text = messageRequestDto.Text;
        await dbContext.SaveChangesAsync(cancellationToken);

        return new(message.Id,
            new(message.Author.Id,
                message.Author.Nickname),
            message.Text,
            message.SentTime);
    }

    public async Task<MessageDto> RemoveMessage(
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

    public async Task<MessagesDto> GetMessages(
        CancellationToken cancellationToken,
        long conversationId,
        long offsetMessageId = 0,
        short limit = 50)
    {
        var currentUser = await userService.GetCurrentUser(cancellationToken);

        var conversation = await dbContext.Conversations
            .Include(x => x.Messages.Where(b => b.Id > offsetMessageId)
                .Take(limit))
            .ThenInclude(message => message.Author)
            .Include(x => x.Participants)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Id == conversationId, cancellationToken);

        var messagesResponses = conversation?.Messages
            .Select(x =>
                new MessageDto(x.Id,
                    new(x.Author.Id, x.Author.Nickname),
                    x.Text,
                    x.SentTime))
            .ToArray();

        if (conversation == null)
        {
            throw new ConversationNotFoundException();
        }

        if (conversation.Participants.All(x => x.Id != currentUser.Id))
        {
            throw new AccessToConversationDeniedException();
        }

        return new(conversation.Id,
            messagesResponses!);
    }
}