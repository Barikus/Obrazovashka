using Microsoft.AspNetCore.Mvc;
using Statistics.Services;
using Statistics.Data;
using Microsoft.Extensions.Logging;

namespace Statistics.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatisticsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<StatisticsController> _logger;

        public StatisticsController(ApplicationDbContext context, ILogger<StatisticsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("global")]
        public IActionResult GetGlobalStatistics()
        {
            _logger.LogInformation("Запрос на получение глобальной статистики");
            var stats = _context.GlobalStatistics!.FirstOrDefault();
            if (stats == null) 
            {
                _logger.LogWarning("Глобальная статистика не найдена.");
                return NotFound("Статистика не найдена.");
            }
            _logger.LogInformation("Глобальная статистика успешно возвращена.");
            return Ok(stats);
        }

        [HttpGet("courses/{courseId}")]
        public IActionResult GetCourseStatistics(int courseId)
        {
            _logger.LogInformation($"Запрос на получение статистики курса ID {courseId}");
            var courseStats = _context.CourseStatistics!.FirstOrDefault(cs => cs.CourseId == courseId);
            if (courseStats == null) 
            {
                _logger.LogWarning($"Статистика курса ID {courseId} не найдена.");
                return NotFound("Статистика курса не найдена.");
            }
            _logger.LogInformation($"Статистика курса ID {courseId} успешно возвращена.");
            return Ok(courseStats);
        }

        [HttpGet("users/{userId}")]
        public IActionResult GetUserStatistics(int userId)
        {
            _logger.LogInformation($"Запрос на получение статистики пользователя ID {userId}");
            var userStats = _context.UserStatistics!.FirstOrDefault(us => us.UserId == userId);
            if (userStats == null) 
            {
                _logger.LogWarning($"Статистика пользователя ID {userId} не найдена.");
                return NotFound("Статистика пользователя не найдена.");
            }
            _logger.LogInformation($"Статистика пользователя ID {userId} успешно возвращена.");
            return Ok(userStats);
        }
    }
}
