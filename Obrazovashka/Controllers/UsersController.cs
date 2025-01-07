using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Obrazovashka.Services;
using Obrazovashka.DTOs;
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
        public async Task<IActionResult> Register([FromBody] UserRegistrationDto registrationDto)
        {
            if (registrationDto == null)
            {
                _logger.LogWarning("Регистрационные данные не могут быть пустыми.");
                return BadRequest("Регистрационные данные не могут быть пустыми.");
            }

            var result = await _userService.RegisterUserAsync(registrationDto);
            if (result.Success)
            {
                _logger.LogInformation($"Пользователь успешно зарегистрирован: {registrationDto.Email}");
                return Ok(result.Message);
            }

            _logger.LogWarning($"Ошибка при регистрации пользователя: {result.Message}");
            return BadRequest(result.Message);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
        {
            if (loginDto == null || string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
            {
                _logger.LogWarning("Данные для входа не могут быть пустыми.");
                return BadRequest("Данные для входа не могут быть пустыми.");
            }

            var result = await _userService.LoginUserAsync(loginDto);
            if (result.Success)
            {
                _logger.LogInformation($"Пользователь {loginDto.Email} успешно вошел в систему.");
                return Ok(new { token = result.Token });
            }

            _logger.LogWarning($"Ошибка при входе: {result.Message}");
            return Unauthorized(result.Message);
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            _logger.LogInformation("Запрос на получение профиля текущего пользователя");
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                _logger.LogWarning("Не удалось получить идентификатор пользователя.");
                return Unauthorized("Не удалось получить идентификатор пользователя.");
            }

            if (!int.TryParse(userIdClaim, out int userId))
            {
                _logger.LogWarning("Некорректный идентификатор пользователя.");
                return BadRequest("Некорректный идентификатор пользователя.");
            }

            var userProfile = await _userService.GetUserByIdAsync(userId);
            if (userProfile == null)
            {
                _logger.LogWarning("Профиль не найден.");
                return NotFound("Профиль не найден.");
            }

            return Ok(userProfile);
        }

        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserProfileDto profileDto)
        {
            _logger.LogInformation("Запрос на обновление профиля текущего пользователя");
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                _logger.LogWarning("Не удалось извлечь идентификатор пользователя из токена.");
                return Unauthorized("Не удалось извлечь идентификатор пользователя из токена.");
            }

            profileDto.Id = userId;

            var result = await _userService.UpdateProfileAsync(profileDto);
            if (result.Success)
            {
                _logger.LogInformation($"Профиль пользователя ID {userId} успешно обновлен.");
                return Ok(result.Message);
            }

            _logger.LogWarning($"Ошибка при обновлении профиля: {result.Message}");
            return BadRequest(result.Message);
        }
    }
}
