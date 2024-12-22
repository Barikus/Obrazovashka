using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Obrazovashka.AuthService.Services;
using Obrazovashka.AuthService.DTOs;

namespace Obrazovashka.AuthService.Controllers
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
            if (registrationDto == null) return 
                    BadRequest("Регистрационные данные не могут быть пустыми.");

            var result = await _userService.RegisterUserAsync(registrationDto);
            if (result.Success ?? false)
                return Ok(result.Message);

            return BadRequest(result.Message);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
        {
            if (loginDto == null) 
                return BadRequest("Данные для входа не могут быть пустыми.");

            var result = await _userService.LoginUserAsync(loginDto);
            if (result.Success ?? false)
                return Ok(new { token = result.Token });

            return Unauthorized(result.Message);
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                _logger.LogWarning("Пользователь не авторизован.");
                return Unauthorized("Не удалось получить идентификатор пользователя.");
            }

            var userId = int.Parse(userIdClaim);
            var userProfile = await _userService.GetUserByIdAsync(userId);
            if (userProfile == null)
            {
                _logger.LogWarning($"Профиль пользователя с ID {userId} не найден.");
                return NotFound("Профиль не найден.");
            }

            return Ok(userProfile);
        }

        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UserProfileUpdateDto profileDto)
        {
            if (profileDto == null) 
                return BadRequest("Данные для обновления профиля не могут быть пустыми.");

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                _logger.LogWarning("Пользователь не авторизован.");
                return Unauthorized("Не удалось получить идентификатор пользователя.");
            }

            var userId = int.Parse(userIdClaim);
            var userResult = await _userService.GetUserByIdAsync(userId);
            if (userResult.Success == true)
            {
                // Обновляем данные профиля
                var userProfile = new UserProfileDto()
                {
                    Email = userResult.User?.Email,
                    Username = userResult.User?.Username
                };

                var profileUpdateResult = await _userService.UpdateProfileAsync(userProfile);
                if (profileUpdateResult.Success ?? false)
                    return Ok(profileUpdateResult.Message);

                return BadRequest(profileUpdateResult.Message);
            }

            _logger.LogWarning($"Профиль пользователя с ID {userId} не найден.");
            return NotFound("Профиль пользователя не найден.");
        }
    }
}
