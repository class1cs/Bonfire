using Bonfire.Application.Interfaces;
using Bonfire.Domain.Dtos.Requests;
using Bonfire.Domain.Dtos.Responses;
using Bonfire.Domain.Entities;
using Bonfire.Domain.Exceptions;
using Bonfire.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Bonfire.Application.Services;

public class ConversationsService(AppDbContext dbContext, IUserService userService) : IConversationsService
{
    public async Task<ConversationDto> CreateConversation(ConversationRequestDto conversationRequestDto, CancellationToken cancellationToken)
    {
        var currentUser = await userService.GetCurrentUser();

        var receivers = await dbContext.Users
            .Where(u => conversationRequestDto.UsersIds.Contains(u.Id))
            .ToListAsync(cancellationToken);

        if (receivers.Count != conversationRequestDto.UsersIds.Count)
            throw new WrongConversationParticipantsIdsException();

        if (receivers.Contains(currentUser)) 
            throw new ReceiverEqualsSenderException();

        receivers.Add(currentUser);

        var participants = new List<User>(receivers);
        var conversationType = receivers.Count > 2 ? ConversationType.Conversation : ConversationType.Dialogue;


        var existingChat = await dbContext.Conversations
            .Include(x => x.Participants)
            .FirstOrDefaultAsync(p => p.Participants
                .All(c => receivers.Contains(c) && p.Type == ConversationType.Dialogue), cancellationToken);

        if (existingChat is not null)
        {
            return new ConversationDto
            (
                existingChat.Id, 
                existingChat.Participants.Select(x => new UserDto
                (
                    x.Id, x.Nickname
                )).ToArray(), existingChat.Type
            );
        }
            
           

        var conversation = new Conversation(new List<Message>(), participants, conversationType);

        await dbContext.Conversations.AddAsync(conversation, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new ConversationDto  
        (
            conversation.Id, 
            conversation.Participants.Select(x => new UserDto
            (
                x.Id, x.Nickname
            )).ToArray(), conversation.Type
        );
    }
    

    public async Task<ConversationDto[]> GetConversations(CancellationToken cancellationToken, long offsetMessageId = 0, short limit = 50)
    {
        var currentUser = await userService.GetCurrentUser();
        var conversations = await dbContext.Conversations
            .AsNoTracking()
            .Include(x => x.Participants)
            .Where(x => x.Participants.Any(x => x.Id == currentUser.Id))
            .Where(b => b.Id > offsetMessageId).OrderByDescending(x => x.Id)
            .Take(limit).ToArrayAsync(cancellationToken);

        var conversationResponses = conversations.Select(x => new ConversationDto
        (
            x.Id,
            x.Participants.Select(user => new UserDto
            (
                user.Id,
                user.Nickname
            )).ToArray(),
            x.Type
        )).ToArray();

        return conversationResponses;
    }
}