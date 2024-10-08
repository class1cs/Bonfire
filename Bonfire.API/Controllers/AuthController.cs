using Bonfire.Application.Interfaces;
using Bonfire.Domain.Dtos.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Bonfire.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IIdentityService _identityService;
    public AuthController(IIdentityService identityService)
    {
        _identityService = identityService;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequestDto registerRequestDto, CancellationToken cancellationToken)
    {
        var responseDto = await _identityService.Register(registerRequestDto, cancellationToken);
        return Ok(responseDto);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDto loginRequestDto, CancellationToken cancellationToken)
    {
        var responseDto = await _identityService.Login(loginRequestDto, cancellationToken);
        return Ok(responseDto);
    }
}