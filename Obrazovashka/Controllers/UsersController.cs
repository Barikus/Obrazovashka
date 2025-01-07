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
                _logger.LogWarning("��������������� ������ �� ����� ���� �������.");
                return BadRequest("��������������� ������ �� ����� ���� �������.");
            }

            var result = await _userService.RegisterUserAsync(registrationDto);
            if (result.Success)
            {
                _logger.LogInformation($"������������ ������� ���������������: {registrationDto.Email}");
                return Ok(result.Message);
            }

            _logger.LogWarning($"������ ��� ����������� ������������: {result.Message}");
            return BadRequest(result.Message);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
        {
            if (loginDto == null || string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
            {
                _logger.LogWarning("������ ��� ����� �� ����� ���� �������.");
                return BadRequest("������ ��� ����� �� ����� ���� �������.");
            }

            var result = await _userService.LoginUserAsync(loginDto);
            if (result.Success)
            {
                _logger.LogInformation($"������������ {loginDto.Email} ������� ����� � �������.");
                return Ok(new { token = result.Token });
            }

            _logger.LogWarning($"������ ��� �����: {result.Message}");
            return Unauthorized(result.Message);
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            _logger.LogInformation("������ �� ��������� ������� �������� ������������");
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                _logger.LogWarning("�� ������� �������� ������������� ������������.");
                return Unauthorized("�� ������� �������� ������������� ������������.");
            }

            if (!int.TryParse(userIdClaim, out int userId))
            {
                _logger.LogWarning("������������ ������������� ������������.");
                return BadRequest("������������ ������������� ������������.");
            }

            var userProfile = await _userService.GetUserByIdAsync(userId);
            if (userProfile == null)
            {
                _logger.LogWarning("������� �� ������.");
                return NotFound("������� �� ������.");
            }

            return Ok(userProfile);
        }

        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserProfileDto profileDto)
        {
            _logger.LogInformation("������ �� ���������� ������� �������� ������������");
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                _logger.LogWarning("�� ������� ������� ������������� ������������ �� ������.");
                return Unauthorized("�� ������� ������� ������������� ������������ �� ������.");
            }

            profileDto.Id = userId;

            var result = await _userService.UpdateProfileAsync(profileDto);
            if (result.Success)
            {
                _logger.LogInformation($"������� ������������ ID {userId} ������� ��������.");
                return Ok(result.Message);
            }

            _logger.LogWarning($"������ ��� ���������� �������: {result.Message}");
            return BadRequest(result.Message);
        }
    }
}
