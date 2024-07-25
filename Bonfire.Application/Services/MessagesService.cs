using Bonfire.Abstractions;
using Bonfire.Core.Dtos.Requests;
using Bonfire.Core.Dtos.Response;
using Bonfire.Core.Entities;
using Bonfire.Core.Exceptions;
using Bonfire.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Bonfire.Application.Services;

public class MessagesService(AppDbContext dbContext, ICurrentUserService currentUserService) : IMessagesService
{
    
    public async Task<MessageResponse> SendMessage(MessageRequest messageRequest, long conversationId)
    {
        var currentUser  = await currentUserService.GetCurrentUser();
        var conversation = await dbContext.Conversations.Include(x => x.Messages)
            .Include(conversation => conversation.Participants).FirstOrDefaultAsync(x => x.Id == conversationId);
        
        if (conversation == null)
        {
            throw new ConversationNotFoundException();
        }

        if (conversation.Participants.All(x => x.Id != currentUser.Id))
        {
            throw new AccessToConversationDeniedException();
        }
        
        if (string.IsNullOrWhiteSpace(messageRequest.Text))
        {
            throw new EmptyMessageTextException();
        }
        
        var message = new Message(messageRequest.Text, DateTime.Now, currentUser);
        
        conversation.Messages.Add(message);
        await dbContext.SaveChangesAsync();
        
        
        return new MessageResponse
        {
            Id = message.Id,
            Text = message.Text
        };
    }
    
    public async Task<MessageResponse> EditMessage(MessageRequest messageRequest, long messageId, long conversationId)
    {
        var currentUser = await currentUserService.GetCurrentUser();
        
        var conversation = await dbContext.Conversations.FirstOrDefaultAsync(x => x.Id == conversationId);
        var message = await dbContext.Messages.Include(message => message.Author).FirstOrDefaultAsync(x => x.Id == messageId && x.ConversationId == conversationId);

        if (conversation is null)
        {
            throw new ConversationNotFoundException();
        }
        
        if (message is null)
        {
            throw new MessageNotFoundException();
        }
        
        if (message?.Author?.Id != currentUser?.Id) 
        {
            throw new AccessToMessageDeniedException();
        }
        
        if (string.IsNullOrWhiteSpace(messageRequest.Text))
        {
            throw new EmptyMessageTextException();
        }
        
        message.Text = messageRequest.Text;
        await dbContext.SaveChangesAsync();
        
        return new MessageResponse
        {
            Id = message.Id,
            Text = message.Text
        };
    }
    
    public async Task<MessageResponse> RemoveMessage(long messageId, long conversationId)
    {
        var currentUser = await currentUserService.GetCurrentUser();
        
        var message = await dbContext.Messages.Include(message => message.Author).FirstOrDefaultAsync(x => x.Id == messageId && x.ConversationId == conversationId);
        
        if (message == null)
        {
            throw new MessageNotFoundException();
        }
        
        if (message?.Author?.Id != currentUser?.Id) 
        {
            throw new AccessToMessageDeniedException();
        }

        dbContext.Messages.Remove(message);
        await dbContext.SaveChangesAsync();
        
        return new MessageResponse
        {
            Id = message.Id,
            Text = message.Text
        };
    }
    
    public async Task<MessagesResponse> GetMessages(long conversationId, long offsetMessageId = 0, short limit = 50)
    {
        var currentUser  = await currentUserService.GetCurrentUser();
        
        var conversation = await dbContext.Conversations.Include(x => x.Messages.Where(b => b.Id > offsetMessageId).Take(limit)).Include(x => x.Participants).FirstOrDefaultAsync(x => x.Id == conversationId);
        var messagesResponses = conversation?.Messages.Select(x => new MessageResponse{Id = x.Id, Text = x.Text}).ToList();
        
        if (conversation == null)
        {
            throw new ConversationNotFoundException();
        }
        
        if (conversation.Participants.All(x => x.Id != currentUser.Id))
        {
            throw new AccessToConversationDeniedException();
        }
        
        return new MessagesResponse
        {
            Id = conversation.Id,
            Messages = messagesResponses!
        };
    }
}
