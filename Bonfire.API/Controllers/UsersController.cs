using Bonfire.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bonfire.API.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class UsersController(IUserInfoService userInfoService) : ControllerBase
{
    [HttpGet("{searchRequest}")]
    public async Task<IActionResult> SearchUsers(string searchRequest)
    {
        var responseDto = await userInfoService.SearchUser(searchRequest);
        return Ok(responseDto);
    }

    [HttpGet]
    public async Task<IActionResult> GetCurrentUserInfo()
    {
        var responseDto = await userInfoService.GetCurrentUserInfo();
        return Ok(responseDto);
    }
}