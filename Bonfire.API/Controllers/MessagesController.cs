using System.Security.Claims;
using Bonfire.Application.Services;
using Bonfire.Core.Dtos.Requests;
using Bonfire.Persistance;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bonfire.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController(MessagesService messagesService, AppDbContext dbContext) : ControllerBase
    {

        [HttpPost("{directChatId}")]
        public async Task<IActionResult> SendMessage(Guid directChatId, [FromBody] MessageRequestDto messageSendRequestDto)
        {
            var responseDto = await messagesService.SendMessage(messageSendRequestDto, directChatId);
            return Ok(responseDto);
        }

        
        [HttpPut("{messageId}")]
        public async Task<IActionResult> EditMessage(Guid messageId, [FromBody] MessageRequestDto messageRequestDto)
        {
            var responseDto = await messagesService.EditMessage(messageRequestDto, messageId);
            return Ok(responseDto);
        }
        
        [HttpDelete("{messageId}")]
        public async Task<IActionResult> RemoveMessage(Guid messageId)
        {
            var responseDto = await messagesService.RemoveMessage(messageId); 
            return Ok(responseDto);
        }
    }
}
