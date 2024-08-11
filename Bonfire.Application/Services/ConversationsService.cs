using Bonfire.Abstractions;
using Bonfire.Core.Dtos.Requests;
using Bonfire.Core.Dtos.Response;
using Bonfire.Core.Entities;
using Bonfire.Core.Exceptions;
using Bonfire.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Bonfire.Application.Services;

public class ConversationsService(AppDbContext dbContext, IUserService userService) : IConversationsService
{
    public async Task<ConversationResponse> CreateConversation(ConversationRequest conversationRequest)
    {
        var currentUser = await userService.GetCurrentUser();
        
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
            .FirstOrDefaultAsync(p => p.Participants.All(c => receivers.Contains(c) && p.Type == ConversationType.Dialogue));
        
        if (existingChat is not null && conversationType == ConversationType.Dialogue)
        {
            return new ConversationResponse
            {
                ConversationType = existingChat.Type,
                Id = existingChat.Id,
                Participants = existingChat.Participants.Select(x => new UserResponse
                {
                    Id = x.Id,
                    NickName = x.Nickname
                }).ToList()
            };
        }
        
        var conversation = new Conversation(new List<Message>(), participants, conversationType);
        
        await dbContext.Conversations.AddAsync(conversation);
        await dbContext.SaveChangesAsync();
        
        return new ConversationResponse
        {
            ConversationType = conversation.Type,
            Id = conversation.Id,
            Participants = conversation.Participants.Select(x => new UserResponse
            {
                Id = x.Id,
                NickName = x.Nickname
            }).ToList()
        };
    }
    
    public async Task<List<ConversationResponse>> GetConversations(long offsetMessageId = 0, short limit = 50)
    {
        var currentUser = await userService.GetCurrentUser();
        var conversations = dbContext.Conversations.AsNoTracking()
            .Include(x => x.Participants)
            .Where(x => x.Participants.Any(x => x.Id == currentUser.Id))
            .Where(b => b.Id > offsetMessageId).OrderByDescending(x => x.Id).Take(limit).ToList();

        var conversationResponses = conversations.Select(x => new ConversationResponse
        {
            ConversationType = x.Type,
            Id = x.Id,
            Participants = x.Participants.Select(x => new UserResponse
            {
                Id = x.Id,
                NickName = x.Nickname
            }).ToList()
        }).ToList();

        return conversationResponses;
    }
}
