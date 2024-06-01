using Bonfire.Abstractions;
using Bonfire.Core.Dtos.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bonfire.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(ILoginService loginService, IRegisterService registerService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDto registerRequestDto)
        {

            var responseDto = await registerService.RegisterAsync(registerRequestDto);
            return Ok(responseDto);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
        {
            var responseDto = await loginService.Login(loginRequestDto);
            return Ok(responseDto);
        }
    }
}
