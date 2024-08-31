using Bonfire.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bonfire.API.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserInfoService _userInfoService;

    public UsersController(IUserInfoService userInfoService)
    {
        _userInfoService = userInfoService;
    }
    
    [HttpGet("{searchRequest}")]
    public async Task<IActionResult> SearchUsers(string searchRequest)
    {
        var responseDto = await _userInfoService.SearchUser(searchRequest);
        return Ok(responseDto);
    }

    [HttpGet]
    public async Task<IActionResult> GetCurrentUserInfo()
    {
        var responseDto = await _userInfoService.GetCurrentUserInfo();
        return Ok(responseDto);
    }
}