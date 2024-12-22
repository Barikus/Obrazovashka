using Obrazovashka.Models;

namespace Obrazovashka.Repositories.Interfaces
{
    public interface ICourseRepository
    {
        Task AddCourseAsync(Course course);
        Task DeleteCourseAsync(int courseId);
        Task<IList<Course>> GetAllCoursesAsync();
        Task<Course> GetCourseByIdAsync(int courseId);
        Task UpdateCourseAsync(Course course);
        Task<IList<Course>> SearchCoursesAsync(string searchTerm);
    }
}
