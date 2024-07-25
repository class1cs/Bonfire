using Bonfire.Abstractions;
using Bonfire.Core.Dtos.Requests;
using Bonfire.Core.Dtos.Response;
using Bonfire.Core.Entities;
using Bonfire.Core.Exceptions;
using Bonfire.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Bonfire.Application.Services;

public class ConversationsService(AppDbContext dbContext, ICurrentUserService currentUserService) : IConversationsService
{
    public async Task<ConversationResponse> CreateConversation(ConversationRequest conversationRequest)
    {
        var currentUser = await currentUserService.GetCurrentUser();
        
        var receivers = await dbContext.Users.Where(u => conversationRequest.UsersIds.Contains(u.Id)).ToListAsync();

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
        var existingChat = await dbContext.Conversations.Include(x => x.Participants)
            .FirstOrDefaultAsync(p => p.Participants.Any(c => receivers.Contains(c) && p.Type == ConversationType.Dialogue));
        
        if (existingChat is not null)
        {
            return new ConversationResponse
            {
                ConversationType = existingChat.Type,
                Id = existingChat.Id,
                Participants = existingChat.Participants.ToList()
            };
        }
        
        var conversation = new Conversation(new List<Message>(), participants, conversationType);
        
        await dbContext.Conversations.AddAsync(conversation);
        await dbContext.SaveChangesAsync();
        
        return new ConversationResponse
        {
            ConversationType = conversation.Type,
            Id = conversation.Id,
            Participants = conversation.Participants.ToList()
        };
    }
    
    public async Task<ConversationResponse> ExitConversation(long conversationId)
    {
        var currentUser = await currentUserService.GetCurrentUser();
        var conversation = await dbContext.Conversations.Include(x => x.Participants).AsNoTracking().FirstOrDefaultAsync(x => x.Id == conversationId);
        
        if (conversation is null)
        {
            throw new ConversationNotFoundException();
        }

        if (conversation.Participants.All(x => x.Id != currentUser.Id))
        {
            throw new AccessToConversationDeniedException();
        }

        conversation.Participants.Remove(currentUser);
        await dbContext.SaveChangesAsync();
        
        return new ConversationResponse
        {
            ConversationType = conversation.Type,
            Id = conversation.Id,
            Participants = conversation.Participants.ToList()
        };
    }
}
