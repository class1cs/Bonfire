using Bonfire.Application.Interfaces;
using Bonfire.Core.Dtos.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Bonfire.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(ILoginService loginService, IRegisterService registerService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest registerRequest)
    {
        var responseDto = await registerService.Register(registerRequest);
        return Ok(responseDto);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest loginRequest)
    {
        var responseDto = await loginService.Login(loginRequest);
        return Ok(responseDto);
    }
}