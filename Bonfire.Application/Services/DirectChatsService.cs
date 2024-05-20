using AutoMapper;
using Bonfire.Core.Dtos.Response;
using Bonfire.Core.Entities;
using Bonfire.Core.Exceptions;
using Bonfire.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Bonfire.Application.Services;

public class DirectChatsService(AppDbContext appDbContext, IMapper mapper)
{
    public async Task<DirectChatResponseDto> CreateDirectChat(Guid recieverId, User currentUser)
    {
        var reciever = await appDbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == recieverId);
        var chatExists = await appDbContext.DirectChats.AsNoTracking().AnyAsync(x =>
            x.Participants.Contains(reciever) && x.Participants.Contains(currentUser));
        if (chatExists)
        {
            throw new DirectChatNotFoundException();
        }
        var users = new List<User>{ reciever, currentUser };
        var directChat = new DirectChat(Guid.NewGuid(), new List<Message>(), users);
        await appDbContext.DirectChats.AddAsync(directChat);
        await appDbContext.SaveChangesAsync();
        var dto = mapper.Map<DirectChatResponseDto>(directChat);
        return dto;
    }
    
    public async Task<DirectChatResponseDto> RemoveDirectChat(Guid directChatId)
    {
        var directChat = await appDbContext.DirectChats.AsNoTracking().FirstOrDefaultAsync(x => x.Id == directChatId);
        if (directChat is null)
        {
            throw new DirectChatNotFoundException();
        }
        appDbContext.DirectChats.Remove(directChat);
        await appDbContext.SaveChangesAsync();
        var dto = mapper.Map<DirectChatResponseDto>(directChat);
        return dto;
    }
}