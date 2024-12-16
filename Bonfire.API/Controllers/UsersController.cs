using Bonfire.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bonfire.API.Controllers;

[Authorize]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserInfoService _userInfoService;

    public UsersController(IUserInfoService userInfoService) => _userInfoService = userInfoService;

    [HttpGet(Routes.Users.SearchUsers)]
    public async Task<IActionResult> SearchUsers(string searchRequest, CancellationToken cancellationToken)
    {
        var responseDto = await _userInfoService.SearchUser(searchRequest, cancellationToken);

        return Ok(responseDto);
    }

    [HttpGet(Routes.Users.GetCurrentUserInfo)]
    public async Task<IActionResult> GetCurrentUserInfo(CancellationToken cancellationToken)
    {
        var responseDto = await _userInfoService.GetCurrentUserInfo(cancellationToken);

        return Ok(responseDto);
    }
}