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

        [HttpPost("{chatId}")]
        public async Task<IActionResult> SendMessage(Guid chatId, [FromBody] SendMessageDto sendMessageDto)
        {
            var currentUserId = ControllerContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "Id")?.Value;
            var currentUserGuidId = Guid.Parse(currentUserId);
            var currentUser = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == currentUserGuidId);
            var responseDto = await messagesService.SendMessage(sendMessageDto, chatId, currentUser);
            return Ok(responseDto);
        }
    }
}
