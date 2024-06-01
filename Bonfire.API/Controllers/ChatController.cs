using System.Security.Claims;
using Bonfire.Application.Services;
using Bonfire.Core.Dtos.Requests;
using Bonfire.Persistance;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bonfire.API.Controllers
{
    [Route("api/chat/directChats")]
    [ApiController]
    public class ChatController(MessagesService messagesService, AppDbContext dbContext, DirectChatsService directChatsService) : ControllerBase
    {

        [HttpPost("{directChatId}/")]
        public async Task<IActionResult> SendMessage(Guid directChatId, [FromBody] MessageRequestDto messageSendRequestDto)
        {
            var responseDto = await messagesService.SendMessage(messageSendRequestDto, directChatId);
            return Ok(responseDto);
        }

        
        [HttpPut("{directChatId}/messages/{messageId}/")]
        public async Task<IActionResult> EditMessage(Guid messageId, Guid directChatId, [FromBody] MessageRequestDto messageRequestDto)
        {
            var responseDto = await messagesService.EditMessage(messageRequestDto, messageId, directChatId);
            return Ok(responseDto);
        }
        
        [HttpDelete("{directChatId}/messages/{messageId}")]
        public async Task<IActionResult> RemoveMessage(Guid messageId, Guid directChatId)
        {
            var responseDto = await messagesService.RemoveMessage(messageId, directChatId); 
            return Ok(responseDto);
        }
        
        [HttpPost("{recieverId}")]
        public async Task<IActionResult> CreateDirectChat(Guid recieverId)
        {
           
            var responseDto = await directChatsService.CreateDirectChat(recieverId);
            return Ok(responseDto);
        }
        
        [HttpDelete("{directChatId}")]
        public async Task<IActionResult> RemoveDirectChat(Guid directChatId)
        {
            var responseDto = await directChatsService.RemoveDirectChat(directChatId);
            return Ok(responseDto);
        }
    }
}
