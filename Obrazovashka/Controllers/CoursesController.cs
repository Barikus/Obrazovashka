using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Obrazovashka.DTOs;
using Obrazovashka.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> CreateCourse([FromBody] CourseCreateDto courseCreateDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            courseCreateDto.AuthorId = userId;

            var result = await _courseService.CreateCourseAsync(courseCreateDto);
            if (result.Success)
            {
                return CreatedAtAction(nameof(GetCourse), new { id = result.CourseId }, result);
            }

            return BadRequest(result);
        }



        [HttpPut("{id}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] CourseUpdateDto courseDto)
        {
            var result = await _courseService.UpdateCourseAsync(id, courseDto);
            if (result.Success)
                return Ok(result);

            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCourses()
        {
            var courses = await _courseService.GetCoursesAsync();
            return Ok(courses);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourse(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course != null)
                return Ok(course);

            return NotFound();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var result = await _courseService.DeleteCourseAsync(id);
            if (result.Success)
                return NoContent();

            return NotFound();
        }
    }
}
