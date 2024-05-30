using AutoMapper;
using Bonfire.Core.Dtos.Requests;
using Bonfire.Core.Dtos.Response;
using Bonfire.Core.Entities;
using Bonfire.Core.Exceptions;
using Bonfire.Persistance;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Bonfire.Application.Services;

public class MessagesService(AppDbContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
{
    public async Task<MessageResponseDto> SendMessage(MessageRequestDto messageSendRequestDto, Guid directChatId)
    {
        var currentUserString = httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == "Id")?.Value;
        var currentUserGuid = Guid.Parse(currentUserString);
        var currentUser = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == currentUserGuid);
        var recieverChat = await dbContext.DirectChats.AsNoTracking().FirstOrDefaultAsync(x => x.Id == directChatId);
        var message = new Message(Guid.NewGuid(), messageSendRequestDto.Text, currentUser, DateTime.Now);
        var userDto = new UserResponseDto(currentUser.Id, currentUser.NickName);
        recieverChat.ChatHistory.Add(message);
        await dbContext.SaveChangesAsync();
        var dto = mapper.Map<MessageResponseDto>(message);
        return dto;
    }
    
    public async Task<MessageResponseDto> EditMessage(MessageRequestDto messageSendRequestDto, Guid messageId)
    {
        var currentUserString = httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == "Id")?.Value;
        var currentUserGuid = Guid.Parse(currentUserString);
        var currentUser = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == currentUserGuid);
        var message = await dbContext.Messages.FirstOrDefaultAsync(x => x.Id == messageId);
        if (message == null)
        {
            throw new MessageNotFoundException();
        }
        if (message?.Author != currentUser) 
        {
            throw new AccessToMessageDeniedException();
        }
        message.Text = messageSendRequestDto.Text;
        await dbContext.SaveChangesAsync();
        var dto = mapper.Map<MessageResponseDto>(message);
        return dto;
    }
    
    public async Task<MessageResponseDto> RemoveMessage(Guid messageId)
    {
        var currentUserString = httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == "Id")?.Value;
        var currentUserGuid = Guid.Parse(currentUserString);
        var currentUser = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == currentUserGuid);
        var message = await dbContext.Messages.FirstOrDefaultAsync(x => x.Id == messageId);
        
        if (message == null)
        {
            throw new MessageNotFoundException();
        }
        
        if (message?.Author != currentUser) 
        {
            throw new AccessToMessageDeniedException();
        }
        
        

        dbContext.Messages.Remove(message);
        await dbContext.SaveChangesAsync();
        var dto = mapper.Map<MessageResponseDto>(message);
        return dto;
    }
}