using Bonfire.Application.Interfaces;
using Bonfire.Domain.Dtos.Requests;
using Bonfire.Domain.Dtos.Responses;
using Bonfire.Domain.Entities;
using Bonfire.Domain.Exceptions;
using Bonfire.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Bonfire.Application.Services;

public class MessagesService(AppDbContext dbContext, IUserService userService) : IMessagesService
{
    public async Task<MessageDto> SendMessage(MessageRequestDto messageRequestDto, long conversationId)
    {
        var currentUser = await userService.GetCurrentUser();
        var conversation = await dbContext.Conversations.Include(x => x.Messages)
            .Include(conversation => conversation.Participants).FirstOrDefaultAsync(x => x.Id == conversationId);

        if (conversation == null) throw new ConversationNotFoundException();

        if (conversation.Participants.All(x => x.Id != currentUser.Id)) throw new AccessToConversationDeniedException();

        if (string.IsNullOrWhiteSpace(messageRequestDto.Text)) throw new EmptyMessageTextException();

        var message = new Message(messageRequestDto.Text, currentUser);

        conversation!.Messages.Add(message);
        await dbContext.SaveChangesAsync();

        return new MessageDto
        (
            message.Id,
            new UserDto 
            (
                message.Author.Id, 
                message.Author.Nickname
            ),
            message.Text,
            message.SentTime
        );
    }

    public async Task<MessageDto> EditMessage(MessageRequestDto messageRequestDto, long messageId, long conversationId)
    {
        var currentUser = await userService.GetCurrentUser();

        var conversation = await dbContext.Conversations.FirstOrDefaultAsync(x => x.Id == conversationId);
        var message = await dbContext.Messages.Include(message => message.Author)
            .FirstOrDefaultAsync(x => x.Id == messageId && x.ConversationId == conversationId);

        if (conversation is null) 
            throw new ConversationNotFoundException();

        if (message is null) 
            throw new MessageNotFoundException();

        if (message.Author.Id != currentUser?.Id)
            throw new AccessToMessageDeniedException();

        if (string.IsNullOrWhiteSpace(messageRequestDto.Text))
            throw new EmptyMessageTextException();
            
        message.Text = messageRequestDto.Text;
        await dbContext.SaveChangesAsync();

        return new MessageDto
        (
            message.Id,
            new UserDto 
            (
                message.Author.Id,
                message.Author.Nickname
            ),
            message.Text,
            message.SentTime
        );
    }

    public async Task<MessageDto> RemoveMessage(long messageId, long conversationId)
    {
        var currentUser = await userService.GetCurrentUser();

        var message = await dbContext.Messages
            .Include(message => message.Author)
            .FirstOrDefaultAsync(x => x.Id == messageId && x.ConversationId == conversationId);

        if (message == null) 
            throw new MessageNotFoundException();

        if (message.Author.Id != currentUser.Id) 
            throw new AccessToMessageDeniedException();

        dbContext.Messages.Remove(message);
        await dbContext.SaveChangesAsync();

        return new MessageDto
        (
            message.Id,
            new UserDto 
            (
                message.Author.Id,
                message.Author.Nickname
            ),
            message.Text,
            message.SentTime
        );
    }

    public async Task<MessagesDto> GetMessages(long conversationId, long offsetMessageId = 0, short limit = 50)
    {
        var currentUser = await userService.GetCurrentUser();

        var conversation = await dbContext.Conversations
            .Include(x => x.Messages.Where(b => b.Id > offsetMessageId).Take(limit))
            .ThenInclude(message => message.Author)
            .Include(x => x.Participants).AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Id == conversationId);
        
        var messagesResponses = conversation?.Messages
            .Select(x =>
                new MessageDto
                (
                    x.Id, 
                    new UserDto(x.Author.Id, x.Author.Nickname),
                    x.Text,
                    x.SentTime
                )).ToArray();

        if (conversation == null) 
            throw new ConversationNotFoundException();

        if (conversation.Participants.All(x => x.Id != currentUser.Id)) 
            throw new AccessToConversationDeniedException();

        return new MessagesDto
        (
            conversation.Id,
            messagesResponses!
        );
    }
}