using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Obrazovashka.DTOs;
using Obrazovashka.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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
            var result = await _userService.RegisterUserAsync(registrationDto);
            if (result.Success)
                return Ok(result.Message);

            return BadRequest(result.Message);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
        {
            var result = await _userService.LoginUserAsync(loginDto);
            if (result.Success)
                return Ok(new { token = result.Token });

            return Unauthorized(result.Message); // ������� 401, ���� ���������
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                _logger.LogWarning("������������ �� �����������.");
                return Unauthorized();
            }

            if (!int.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning("�������� ������������� ������������: {UserIdClaim}", userIdClaim);
                return BadRequest("�������� ������������� ������������.");
            }

            var userProfile = await _userService.GetUserByIdAsync(userId);
            if (userProfile == null)
            {
                _logger.LogWarning("������� ������������ � ID {UserId} �� ������.", userId);
                return NotFound();
            }

            return Ok(userProfile);
        }


        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UserProfileDto profileDto)
        {
            if (profileDto == null)
            {
                return BadRequest("������� ������.");
            }

            var result = await _userService.UpdateProfileAsync(profileDto);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result);
        }

    }
}
