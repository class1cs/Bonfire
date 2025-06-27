using Bonfire.Application.Interfaces;
using Bonfire.Domain.Dtos.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bonfire.API.Controllers;

[AllowAnonymous]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IIdentityService _identityService;

    public AuthController(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    [HttpPost(Routes.Auth.Register)]
    public async Task<IActionResult> Register(RegisterRequest registerRequest, CancellationToken cancellationToken)
    {
        var responseDto = await _identityService.Register(registerRequest, cancellationToken);

        return Ok(responseDto);
    }

    [HttpPost(Routes.Auth.Login)]
    public async Task<IActionResult> Login(LoginRequest loginRequest, CancellationToken cancellationToken)
    {
        var responseDto = await _identityService.Login(loginRequest, cancellationToken);

        return Ok(responseDto);
    }
}