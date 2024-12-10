using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Obrazovashka.DTOs;
using Obrazovashka.Services;

namespace Obrazovashka.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentsController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        [HttpPost]
        public async Task<IActionResult> Enroll([FromBody] EnrollmentDto enrollmentDto)
        {
            var result = await _enrollmentService.EnrollInCourseAsync(enrollmentDto);
            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpPost("{courseId}/feedback")]
        public async Task<IActionResult> LeaveFeedback(int courseId, [FromBody] FeedbackDto feedbackDto)
        {
            var result = await _enrollmentService.LeaveFeedbackAsync(courseId, feedbackDto);
            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }
    }
}
