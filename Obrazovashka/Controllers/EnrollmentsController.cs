using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Obrazovashka.DTOs;
using Obrazovashka.Services.Interfaces;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

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
            _logger.LogInformation("Запрос на запись на курс");
            if (enrollmentDto == null || enrollmentDto.CourseId == null)
            {
                _logger.LogWarning("Недостаточно данных для записи.");
                return BadRequest("Недостаточно данных для записи.");
            }

            var result = await _enrollmentService.EnrollInCourseAsync(enrollmentDto);
            if (result.Success)
            {
                _logger.LogInformation($"Успешная запись на курс ID {enrollmentDto.CourseId}");
                return Ok(result);
            }

            _logger.LogWarning($"Не удалось записаться на курс ID {enrollmentDto.CourseId}: {result.Message}");
            return BadRequest(result);
        }

        [HttpGet("my")]
        [Authorize]
        public async Task<IActionResult> GetMyEnrollments()
        {
            _logger.LogInformation("Запрос на получение записей текущего пользователя");
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                _logger.LogWarning("Пользователь не авторизован.");
                return Unauthorized("Вы не авторизованы.");
            }

            if (!int.TryParse(userIdClaim, out int userId))
            {
                _logger.LogWarning("Некорректный идентификатор пользователя.");
                return BadRequest("Некорректный идентификатор пользователя.");
            }

            var enrollments = await _enrollmentService.GetUserEnrollmentsAsync(userId);
            return Ok(enrollments);
        }

        [HttpPost("{courseId}/feedback")]
        [Authorize]
        public async Task<IActionResult> LeaveFeedback(int courseId, [FromBody] FeedbackDto feedbackDto)
        {
            _logger.LogInformation($"Запрос на отзыв для курса ID {courseId}");
            if (feedbackDto == null || feedbackDto.Rating < 1 || feedbackDto.Rating > 5)
            {
                _logger.LogWarning("Недопустимый формат отзыва или оценки.");
                return BadRequest("Недопустимый формат отзыва или оценки.");
            }

            var result = await _enrollmentService.LeaveFeedbackAsync(courseId, feedbackDto);
            if (result.Success)
            {
                _logger.LogInformation($"Отзыв успешно оставлен для курса ID {courseId}");
                return Ok(result);
            }

            _logger.LogWarning($"Не удалось оставить отзыв для курса ID {courseId}: {result.Message}");
            return BadRequest(result);
        }
    }
}
