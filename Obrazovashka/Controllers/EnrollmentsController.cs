using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Obrazovashka.DTOs;
using Obrazovashka.Services.Interfaces;

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
            var result = await _enrollmentService.EnrollInCourseAsync(enrollmentDto);
            if (result.Success ?? false)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpPost("{courseId}/feedback")]
        [Authorize]
        public async Task<IActionResult> LeaveFeedback(int courseId, [FromBody] FeedbackDto feedbackDto)
        {
            var result = await _enrollmentService.LeaveFeedbackAsync(courseId, feedbackDto);
            if (result.Success ?? false)
                return Ok(result);

            return BadRequest(result);
        }
    }
}
