using System.Collections.Generic;
using System.Threading.Tasks;
using Obrazovashka.Models;

namespace Obrazovashka.Repositories.Interfaces
{
    public interface ICourseRepository
    {
        Task AddCourseAsync(Course course);
        Task DeleteCourseAsync(int id);
        Task<IEnumerable<Course>> GetAllCoursesAsync();
        Task<Course> GetCourseByIdAsync(int id);
        Task UpdateCourseAsync(Course course);
    }
}
