using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Obrazovashka.DTOs;
using Obrazovashka.Services;

namespace Obrazovashka.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] CourseCreateDto courseDto)
        {
            var result = await _courseService.CreateCourseAsync(courseDto);
            if (result.Success)
                return CreatedAtAction(nameof(GetCourse), new { id = result.CourseId }, result);

            return BadRequest(result);
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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] CourseUpdateDto courseDto)
        {
            var result = await _courseService.UpdateCourseAsync(id, courseDto);
            if (result.Success)
                return Ok(result);

            return NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var result = await _courseService.DeleteCourseAsync(id);
            if (result.Success)
                return NoContent();

            return NotFound();
        }
    }
}
