using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Obrazovashka.DTOs;
using Obrazovashka.Services;
using Microsoft.Extensions.Logging;

namespace Obrazovashka.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] UserRegistrationDto registrationDto)
        {
            var result = await _userService.RegisterUserAsync(registrationDto);
            if (result.Success)
                return Ok(result.Message);

            return BadRequest(result.Message);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] UserLoginDto loginDto)
        {
            var result = await _userService.LoginUserAsync(loginDto);
            if (result.Success)
                return Ok(new { token = result.Token });

            return Unauthorized(result.Message);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserProfileDto profileDto)
        {
            var result = await _userService.UpdateProfileAsync(profileDto);
            if (result.Success)
                return Ok(result);

            return BadRequest(result.Message);
        }
    }
}
