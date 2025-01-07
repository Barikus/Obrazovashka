using System.Collections.Generic;
using System.Threading.Tasks;
using Obrazovashka.DTOs;
using Obrazovashka.Models;
using Obrazovashka.Results;

namespace Obrazovashka.Services.Interfaces
{
    public interface ICourseService
    {
        Task<CourseCreateResult> CreateCourseAsync(CourseDto courseDto);
        Task<IList<CourseDto>> GetCoursesAsync();
        Task<IList<CourseDto>> GetCoursesByUserIdAsync(int userId);
        Task<CourseDto> GetCourseByIdAsync(int courseId);
        Task<IList<string>> GetCourseContentsAsync(string courseFolderPath);
        Task<IList<string>> GetCourseFilesAsync(string courseFolderPath);
        Task<CourseUpdateResult> UpdateCourseAsync(int id, CourseUpdateDto courseDto);
        Task<DeletionResult> DeleteCourseAsync(int courseId);
        Task<string> SaveFileAsync(IFormFile file, string courseFolderPath);
        Task<DeletionResult> DeleteFileAsync(int courseId, string fileName);
        Task<IList<CourseDto>> SearchCoursesAsync(string searchTerm);
        Task<IList<CourseDto>> SearchAndFilterCoursesAsync(string searchTerm, string[]? tags);
        Task<IList<CourseDto>> RecommendCoursesAsync(int userId);
    }
}
