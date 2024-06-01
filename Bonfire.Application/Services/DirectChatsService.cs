using AutoMapper;
using Bonfire.Core.Dtos.Response;
using Bonfire.Core.Entities;
using Bonfire.Core.Exceptions;
using Bonfire.Persistance;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Bonfire.Application.Services;

public class DirectChatsService(AppDbContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor, ILogger<MessagesService> logger)
{
    public async Task<DirectChatResponseDto> CreateDirectChat(Guid recieverId)
    {
        var currentUserString = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "Id")?.Value;
        var currentUserGuid = Guid.Parse(currentUserString);
        var currentUser = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == currentUserGuid);
        
        var reciever = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == recieverId);
        if (reciever == currentUser)
        {
            throw new ReceiverEqualsSenderException();
        }
        var chatExists = await dbContext.DirectChats.AsNoTracking().AnyAsync(x =>
            x.Participants.Contains(reciever) && x.Participants.Contains(currentUser));
        if (chatExists)
        {
            throw new DirectChatAlreadyExistsException();
        }
        
        var users = new List<User>{ reciever, currentUser };
        var directChat = new DirectChat(new List<Message>(), users);
        await dbContext.DirectChats.AddAsync(directChat);
        await dbContext.SaveChangesAsync();
        
        var dto = mapper.Map<DirectChatResponseDto>(directChat);
        return dto;
    }
    
    public async Task<DirectChatResponseDto> RemoveDirectChat(Guid directChatId)
    {
        var currentUserString = httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == "Id")?.Value;
        var currentUserGuid = Guid.Parse(currentUserString);
        var currentUser = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == currentUserGuid);
        
        var directChat = await dbContext.DirectChats.Include(x => x.Participants).AsNoTracking().FirstOrDefaultAsync(x => x.Id == directChatId);
        if (directChat is null)
        {
            throw new DirectChatNotFoundException();
        }

        if (!directChat.Participants.Contains(currentUser))
        {
            throw new AccessToDirectChatDeniedException();
        }
        dbContext.DirectChats.Remove(directChat);
        await dbContext.SaveChangesAsync();
        
        var dto = mapper.Map<DirectChatResponseDto>(directChat);
        return dto;
    }
}