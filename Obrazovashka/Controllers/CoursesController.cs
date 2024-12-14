using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Obrazovashka.DTOs;
using Obrazovashka.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using System.IO;

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
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
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
                Title = courseCreateDto.Title,
                Description = courseCreateDto.Description,
                Tags = courseCreateDto.Tags,
                AuthorId = userId,
                ContentPath = courseFolderPath
            };

            var result = await _courseService.CreateCourseAsync(courseDto);
            if (result.Success)
            {
                return CreatedAtAction(nameof(GetCourse), new { id = result.CourseId }, result);
            }

            return BadRequest(result);
        }

        // Получение курса по ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourse(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
 
            return Ok(course);
        }

        // Получение содержимого курса
        [HttpGet("{id}/contents")]
        public async Task<IActionResult> GetCourseContents(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null)
                return NotFound("Курс не найден.");

            var contents = await _courseService.GetCourseContentsAsync(course.ContentPath);
            return Ok(contents);
        }

        // Получение файлов курса
        [HttpGet("{id}/files")]
        public async Task<IActionResult> GetCourseFiles(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null)
                return NotFound("Курс не найден.");

            var existingFiles = await _courseService.GetCourseFilesAsync(course.ContentPath);
            return Ok(existingFiles);
        }

        // Удаление файла из курса
        [HttpDelete("{id}/files/{fileName}")]
        public async Task<IActionResult> DeleteFile(int id, string fileName)
        {
            var result = await _courseService.DeleteFileAsync(id, fileName);
            if (result.Success)
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
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var courses = await _courseService.GetCoursesByUserIdAsync(userId);
            return Ok(courses);
        }

        // Обновление курса
        [HttpPut("{id}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> UpdateCourse(int id, [FromForm] CourseUpdateDto courseUpdateDto, IList<IFormFile> contentFiles)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (course == null || course.AuthorId != userId)
            {
                return NotFound("Курс не найден или у вас нет прав доступа.");
            }

            courseUpdateDto.ContentFiles = contentFiles; // Добавляем файлы к DTO для обновления
            var result = await _courseService.UpdateCourseAsync(id, courseUpdateDto);
            if (result.Success)
            {
                return Ok(result);
            }

            return NotFound("Курс не найден.");
        }

        // Удаление курса
        [HttpDelete("{id}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var result = await _courseService.DeleteCourseAsync(id);
            if (result.Success)
                return NoContent();

            return NotFound("Курс не найден.");
        }
    }
}
