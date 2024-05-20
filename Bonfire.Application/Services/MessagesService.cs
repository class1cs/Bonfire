using AutoMapper;
using Bonfire.Core.Dtos.Requests;
using Bonfire.Core.Dtos.Response;
using Bonfire.Core.Entities;
using Bonfire.Core.Exceptions;
using Bonfire.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Bonfire.Application.Services;

public class MessagesService(AppDbContext dbContext, IMapper mapper)
{
    public async Task<MessageResponseDto> SendMessage(MessageRequestDto messageSendRequestDto, Guid directChatId, User author)
    {
        var recieverChat = await dbContext.DirectChats.AsNoTracking().FirstOrDefaultAsync(x => x.Id == directChatId);
        var message = new Message(Guid.NewGuid(), messageSendRequestDto.Text, author, DateTime.Now);
        var userDto = new UserResponseDto(author.Id, author.NickName);
        recieverChat.ChatHistory.Add(message);
        await dbContext.SaveChangesAsync();
        var dto = mapper.Map<MessageResponseDto>(message);
        return dto;
    }
    
    public async Task<MessageResponseDto> EditUserOwnMessage(MessageRequestDto messageSendRequestDto, Guid messageId, User author)
    {
        var message = await dbContext.Messages.FirstOrDefaultAsync(x => x.Id == messageId);
        if (message == null)
        {
            throw new MessageNotFoundException();
        }
        if (message?.Author != author) 
        {
            throw new AccessToMessageDeniedException();
        }
        message.Text = messageSendRequestDto.Text;
        await dbContext.SaveChangesAsync();
        var dto = mapper.Map<MessageResponseDto>(message);
        return dto;
    }
    
    public async Task<MessageResponseDto> RemoveUserOwnMessage(Guid messageId, User author)
    {
        var message = await dbContext.Messages.FirstOrDefaultAsync(x => x.Id == messageId);
        
        if (message == null)
        {
            throw new MessageNotFoundException();
        }
        
        if (message?.Author != author) 
        {
            throw new AccessToMessageDeniedException();
        }

        dbContext.Messages.Remove(message);
        await dbContext.SaveChangesAsync();
        var dto = mapper.Map<MessageResponseDto>(message);
        return dto;
    }
}