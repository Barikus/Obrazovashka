using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Obrazovashka.DTOs;
using Obrazovashka.Services.Interfaces;

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
            if (courseCreateDto == null)
                return BadRequest();
            if (contentFiles == null || contentFiles.Count == 0)
                return BadRequest("Необходимо загрузить хотя бы один файл.");

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
            if (result.Success ?? false)
            {
                return CreatedAtAction(nameof(GetCourse), new { id = result.CourseId }, result);
            }

            return BadRequest(result.Message);
        }

        // Получение курса по ID
        [HttpGet("{courseId}")]
        public async Task<IActionResult> GetCourse(int courseId)
        {
            var course = await _courseService.GetCourseByIdAsync(courseId);
            if (course == null) 
                return NotFound($"Курс с ID {courseId} не найден.");

            return Ok(course);
        }

        // Получение содержимого курса
        [HttpGet("{courseId}/contents")]
        public async Task<IActionResult> GetCourseContents(int courseId)
        {
            var course = await _courseService.GetCourseByIdAsync(courseId);
            if (course == null)
                return NotFound("Курс не найден.");

            var contents = await _courseService.GetCourseContentsAsync(course.ContentPath ?? string.Empty);
            return Ok(contents);
        }

        // Получение файлов курса
        [HttpGet("{courseId}/files")]
        public async Task<IActionResult> GetCourseFiles(int courseId)
        {
            var course = await _courseService.GetCourseByIdAsync(courseId);
            if (course == null)
                return NotFound("Курс не найден.");

            var existingFiles = await _courseService.GetCourseFilesAsync(course.ContentPath ?? string.Empty);
            return Ok(existingFiles);
        }

        // Удаление файла из курса
        [HttpDelete("{courseId}/files/{fileName}")]
        public async Task<IActionResult> DeleteFile(int id, string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return BadRequest("Имя файла не может быть пустым.");

            var result = await _courseService.DeleteFileAsync(id, fileName);
            if (result.Success ?? false)
                return NoContent();

            return NotFound(result.Message);
        }

        // Получение всех курсов
        [HttpGet("all")]
        public async Task<IActionResult> GetAllCourses()
        {
            var courses = await _courseService.GetCoursesAsync();
            return Ok(courses);
        }

        // Получение курсов текущего пользователя
        [HttpGet("my")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetMyCourses()
        {
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
            if (courseUpdateDto == null) return BadRequest();

            var course = await _courseService.GetCourseByIdAsync(courseId);
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("Не удалось получить идентификатор пользователя.");
            var userId = int.Parse(userIdClaim);

            if (course == null || course.AuthorId != userId)
            {
                return NotFound("Курс не найден или у вас нет прав доступа.");
            }

            courseUpdateDto.ContentFiles = contentFiles;
            var result = await _courseService.UpdateCourseAsync(courseId, courseUpdateDto);
            if (result.Success ?? false)
            {
                return Ok(result.Message);
            }

            return NotFound("Курс не найден.");
        }

        // Удаление курса
        [HttpDelete("{courseId}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> DeleteCourse(int courseId)
        {
            var result = await _courseService.DeleteCourseAsync(courseId);
            if (!(result.Success ?? false))
                return NotFound(result.Message);

            return NoContent();
        }
    }
}
