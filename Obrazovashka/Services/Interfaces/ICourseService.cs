using System.Collections.Generic;
using System.Threading.Tasks;
using Obrazovashka.DTOs;
using Obrazovashka.Results;

namespace Obrazovashka.Services
{
    public interface ICourseService
    {
        Task<CourseCreateResult> CreateCourseAsync(CourseCreateDto courseDto);
        Task<IEnumerable<CourseDto>> GetCoursesAsync();
        Task<CourseDto> GetCourseByIdAsync(int id);
        Task<CourseUpdateResult> UpdateCourseAsync(int id, CourseUpdateDto courseDto);
        Task<DeletionResult> DeleteCourseAsync(int id);
    }
}
