using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Obrazovashka.DTOs;
using Obrazovashka.Services.Interfaces;
using System.Security.Claims;

namespace Obrazovashka.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly ILogger<EnrollmentsController> _logger;

        public EnrollmentsController(IEnrollmentService enrollmentService, ILogger<EnrollmentsController> logger)
        {
            _enrollmentService = enrollmentService;
            _logger = logger;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Enroll([FromBody] EnrollmentDto enrollmentDto)
        {
            if (enrollmentDto == null || enrollmentDto.CourseId == null)
                return BadRequest("Недостаточно данных для записи.");

            var result = await _enrollmentService.EnrollInCourseAsync(enrollmentDto);
            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpGet("my")]
        [Authorize]
        public async Task<IActionResult> GetMyEnrollments()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized("Вы не авторизованы.");

            if (!int.TryParse(userIdClaim, out int userId))
                return BadRequest("Некорректный идентификатор пользователя.");

            var enrollments = await _enrollmentService.GetUserEnrollmentsAsync(userId);
            return Ok(enrollments);
        }

        [HttpPost("{courseId}/feedback")]
        [Authorize]
        public async Task<IActionResult> LeaveFeedback(int courseId, [FromBody] FeedbackDto feedbackDto)
        {
            if (feedbackDto == null || feedbackDto.Rating < 1 || feedbackDto.Rating > 5)
                return BadRequest("Недопустимый формат отзыва или оценки.");

            var result = await _enrollmentService.LeaveFeedbackAsync(courseId, feedbackDto);
            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }
    }
}
