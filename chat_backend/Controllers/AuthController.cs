using Microsoft.AspNetCore.Mvc;
using _NET_Realtime_Chat.Models;
using _NET_Realtime_Chat.Services;
using System.Threading.Tasks;

namespace _NET_Realtime_Chat.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!await _userService.RegisterUser(model))
                return BadRequest("User already exists.");

            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var token = await _userService.Authenticate(model);
            if (token == null)
                return Unauthorized();

            return Ok(new { Token = token });
        }
    }
}
