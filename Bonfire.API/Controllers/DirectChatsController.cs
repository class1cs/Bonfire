using Bonfire.Application.Services;
using Bonfire.Persistance;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bonfire.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DirectChatsController(AppDbContext dbContext, DirectChatsService directChatsService) : ControllerBase
    {  
        [HttpPost("{recieverId}")]
        public async Task<IActionResult> CreateDirectChat(Guid recieverId)
        {
            var currentUserId = ControllerContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "Id")?.Value;
            var currentUserGuidId = Guid.Parse(currentUserId);
            var currentUser = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == currentUserGuidId);
            var responseDto = await directChatsService.CreateDirectChat(recieverId, currentUser);
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
