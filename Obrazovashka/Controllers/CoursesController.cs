using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Obrazovashka.DTOs;
using Obrazovashka.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Obrazovashka.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly ILogger<CoursesController> _logger;

        public CoursesController(ICourseService courseService, ILogger<CoursesController> logger)
        {
            _courseService = courseService;
            _logger = logger;
        }

        // Создание нового курса
        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> CreateCourse([FromForm] CourseCreateDto courseCreateDto, IList<IFormFile> contentFiles)
        {
            _logger.LogInformation("Запрос на создание нового курса");
            if (courseCreateDto == null)
                return BadRequest();
            if (contentFiles == null || contentFiles.Count == 0)
            {
                _logger.LogWarning("Необходимо загрузить хотя бы один файл.");
                return BadRequest("Необходимо загрузить хотя бы один файл.");
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                _logger.LogWarning("Пользователь не авторизован.");
                return Unauthorized("Не удалось получить идентификатор пользователя.");
            }

            var userId = int.Parse(userIdClaim);
            var courseFolderPath = Path.Combine("Content", Guid.NewGuid().ToString());
            Directory.CreateDirectory(courseFolderPath);

            // Сохраняем загруженные файлы
            foreach (var file in contentFiles)
            {
                if (file.Length > 0)
                {
                    await _courseService.SaveFileAsync(file, courseFolderPath);
                }
            }

            var courseDto = new CourseDto
            {
                Title = courseCreateDto.Title ?? string.Empty,
                Description = courseCreateDto.Description ?? string.Empty,
                Tags = courseCreateDto.Tags ?? Array.Empty<string>(),
                AuthorId = userId,
                ContentPath = courseFolderPath
            };

            var result = await _courseService.CreateCourseAsync(courseDto);
            if (result.Success)
            {
                _logger.LogInformation($"Курс '{courseDto.Title}' успешно создан. ID курса: {result.CourseId}");
                return CreatedAtAction(nameof(GetCourse), new { id = result.CourseId }, result);
            }

            _logger.LogWarning($"Ошибка при создании курса: {result.Message}");
            return BadRequest(result.Message);
        }

        // Получение курса по ID
        [HttpGet("{courseId}")]
        public async Task<IActionResult> GetCourse(int courseId)
        {
            _logger.LogInformation($"Запрос на получение курса ID {courseId}");
            var course = await _courseService.GetCourseByIdAsync(courseId);
            if (course == null)
            {
                _logger.LogWarning($"Курс с ID {courseId} не найден.");
                return NotFound($"Курс с ID {courseId} не найден.");
            }

            return Ok(course);
        }

        // Получение содержимого курса
        [HttpGet("{courseId}/contents")]
        public async Task<IActionResult> GetCourseContents(int courseId)
        {
            _logger.LogInformation($"Запрос на получение содержимого курса ID {courseId}");
            var course = await _courseService.GetCourseByIdAsync(courseId);
            if (course == null)
            {
                _logger.LogWarning("Курс не найден.");
                return NotFound("Курс не найден.");
            }

            var contents = await _courseService.GetCourseContentsAsync(course.ContentPath ?? string.Empty);
            return Ok(contents);
        }

        // Получение файлов курса
        [HttpGet("{courseId}/files")]
        public async Task<IActionResult> GetCourseFiles(int courseId)
        {
            _logger.LogInformation($"Запрос на получение файлов курса ID {courseId}");
            var course = await _courseService.GetCourseByIdAsync(courseId);
            if (course == null)
            {
                _logger.LogWarning("Курс не найден.");
                return NotFound("Курс не найден.");
            }

            var existingFiles = await _courseService.GetCourseFilesAsync(course.ContentPath ?? string.Empty);
            return Ok(existingFiles);
        }

        // Удаление файла из курса
        [HttpDelete("{courseId}/files/{fileName}")]
        public async Task<IActionResult> DeleteFile(int id, string fileName)
        {
            _logger.LogInformation($"Запрос на удаление файла '{fileName}' из курса ID {id}");
            if (string.IsNullOrEmpty(fileName))
            {
                _logger.LogWarning("Имя файла не может быть пустым.");
                return BadRequest("Имя файла не может быть пустым.");
            }

            var result = await _courseService.DeleteFileAsync(id, fileName);
            if (result.Success)
            {
                _logger.LogInformation($"Файл '{fileName}' успешно удален из курса ID {id}");
                return NoContent();
            }

            _logger.LogWarning(result.Message);
            return NotFound(result.Message);
        }

        // Получение всех курсов
        [HttpGet("all")]
        public async Task<IActionResult> GetAllCourses()
        {
            _logger.LogInformation("Запрос на получение всех курсов");
            var courses = await _courseService.GetCoursesAsync();
            return Ok(courses);
        }

        // Получение курсов текущего пользователя
        [HttpGet("my")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetMyCourses()
        {
            _logger.LogInformation("Запрос на получение курсов текущего пользователя");
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                _logger.LogWarning("Пользователь не авторизован.");
                return Unauthorized("Не удалось получить идентификатор пользователя.");
            }

            var userId = int.Parse(userIdClaim);
            var courses = await _courseService.GetCoursesByUserIdAsync(userId);
            return Ok(courses);
        }

        // Обновление курса
        [HttpPut("{courseId}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> UpdateCourse(int courseId, [FromForm] CourseUpdateDto courseUpdateDto, IList<IFormFile> contentFiles)
        {
            _logger.LogInformation($"Запрос на обновление курса ID {courseId}");
            if (courseUpdateDto == null)
            {
                _logger.LogWarning("Обновление курса: недопустимые данные.");
                return BadRequest();
            }

            var course = await _courseService.GetCourseByIdAsync(courseId);
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                _logger.LogWarning("Пользователь не авторизован.");
                return Unauthorized("Не удалось получить идентификатор пользователя.");
            }
            var userId = int.Parse(userIdClaim);

            if (course == null || course.AuthorId != userId)
            {
                _logger.LogWarning("Курс не найден или у вас нет прав доступа.");
                return NotFound("Курс не найден или у вас нет прав доступа.");
            }

            courseUpdateDto.ContentFiles = contentFiles;
            var result = await _courseService.UpdateCourseAsync(courseId, courseUpdateDto);
            if (result.Success)
            {
                _logger.LogInformation($"Курс ID {courseId} успешно обновлен.");
                return Ok(result.Message);
            }

            _logger.LogWarning("Курс не найден.");
            return NotFound("Курс не найден.");
        }

        // Удаление курса
        [HttpDelete("{courseId}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> DeleteCourse(int courseId)
        {
            _logger.LogInformation($"Запрос на удаление курса ID {courseId}");
            var result = await _courseService.DeleteCourseAsync(courseId);
            if (!result.Success)
            {
                _logger.LogWarning(result.Message);
                return NotFound(result.Message);
            }

            _logger.LogInformation($"Курс ID {courseId} успешно удален.");
            return NoContent();
        }

        // Поиск курса
        [HttpGet("search")]
        public async Task<IActionResult> SearchCourses(string searchTerm)
        {
            _logger.LogInformation($"Запрос на поиск курсов по запросу '{searchTerm}'");
            var courses = await _courseService.SearchCoursesAsync(searchTerm);
            return Ok(courses);
        }
    }
}
