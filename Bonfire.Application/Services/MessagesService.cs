using Bonfire.Core.Dtos.Requests;
using Bonfire.Core.Dtos.Response;
using Bonfire.Core.Entities;
using Bonfire.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Bonfire.Application.Services;

public class MessagesService(AppDbContext dbContext)
{
    public async Task<MessageResponseDto> SendMessage(SendMessageDto sendMessageDto, Guid chatId, User author)
    {
        var recieverChat = await dbContext.DirectChats.AsNoTracking().FirstOrDefaultAsync(x => x.Id == chatId);
        var message = new Message(Guid.NewGuid(), sendMessageDto.Text, author, DateTime.Now);
        var userDto = new UserResponseDto(author.Id, author.NickName);
        recieverChat.ChatHistory.Add(message);
        await dbContext.SaveChangesAsync();
        return new MessageResponseDto(Guid.NewGuid(), sendMessageDto.Text, userDto);


    }
}