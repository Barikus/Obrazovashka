using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Obrazovashka.Services;
using Obrazovashka.DTOs;

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
        public async Task<IActionResult> Register([FromBody] UserRegistrationDto registrationDto)
        {
            if (registrationDto == null)
                return BadRequest("Регистрационные данные не могут быть пустыми.");

            var result = await _userService.RegisterUserAsync(registrationDto);
            if (result.Success == true)
                return Ok(result.Message);

            return BadRequest(result.Message);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
        {
            if (loginDto == null || string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
                return BadRequest("Данные для входа не могут быть пустыми.");

            var result = await _userService.LoginUserAsync(loginDto);
            if (result.Success == true)
                return Ok(new { token = result.Token });

            return Unauthorized(result.Message);
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("Не удалось получить идентификатор пользователя.");

            if (!int.TryParse(userIdClaim, out int userId))
                return BadRequest("Некорректный идентификатор пользователя.");

            var userProfile = await _userService.GetUserByIdAsync(userId);
            if (userProfile == null)
                return NotFound("Профиль не найден.");

            return Ok(userProfile);
        }

        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserProfileDto profileDto)
        {
            // Получение userId из токена
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("Не удалось извлечь идентификатор пользователя из токена.");
            }

            profileDto.Id = userId;

            var result = await _userService.UpdateProfileAsync(profileDto);
            if (result.Success)
            {
                return Ok(result.Message);
            }

            return BadRequest(result.Message);
        }

    }
}
