using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Obrazovashka.DTOs;
using Obrazovashka.Services;

namespace Obrazovashka.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegistrationDto registrationDto)
        {
            var result = await _userService.RegisterUserAsync(registrationDto);
            if (result.Success)
                return Ok(result.Message);

            return BadRequest(result.Message);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto loginDto)
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
