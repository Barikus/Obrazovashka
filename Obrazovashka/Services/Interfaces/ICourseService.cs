using System.Collections.Generic;
using System.Threading.Tasks;
using Obrazovashka.DTOs;
using Obrazovashka.Results;

namespace Obrazovashka.Services
{
    public interface ICourseService
    {
        Task<CourseCreateResult> CreateCourseAsync(CourseDto courseDto);
        Task<IEnumerable<CourseDto>> GetCoursesAsync();
        Task<IEnumerable<CourseDto>> GetCoursesByUserIdAsync(int userId);
        Task<CourseDto> GetCourseByIdAsync(int id);
        Task<IEnumerable<string>> GetCourseContentsAsync(string courseFolderPath);
        Task<IEnumerable<string>> GetCourseFilesAsync(string courseFolderPath);
        Task<CourseUpdateResult> UpdateCourseAsync(int id, CourseUpdateDto courseDto);
        Task<DeletionResult> DeleteCourseAsync(int id);
        Task<string> SaveFileAsync(IFormFile file, string courseFolderPath);
        Task<DeletionResult> DeleteFileAsync(int courseId, string fileName);
    }
}
