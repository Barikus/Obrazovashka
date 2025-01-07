using Microsoft.AspNetCore.Mvc;
using Statistics.Services;
using Statistics.Data;

namespace Statistics.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatisticsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StatisticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("global")]
        public IActionResult GetGlobalStatistics()
        {
            var stats = _context.GlobalStatistics!.FirstOrDefault();
            if (stats == null) return NotFound("Статистика не найдена.");
            return Ok(stats);
        }

        [HttpGet("courses/{courseId}")]
        public IActionResult GetCourseStatistics(int courseId)
        {
            var courseStats = _context.CourseStatistics!.FirstOrDefault(cs => cs.CourseId == courseId);
            if (courseStats == null) return NotFound("Статистика курса не найдена.");
            return Ok(courseStats);
        }

        [HttpGet("users/{userId}")]
        public IActionResult GetUserStatistics(int userId)
        {
            var userStats = _context.UserStatistics!.FirstOrDefault(us => us.UserId == userId);
            if (userStats == null) return NotFound("Статистика пользователя не найдена.");
            return Ok(userStats);
        }
    }
}
