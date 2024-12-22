using Microsoft.EntityFrameworkCore;
using Obrazovashka.Data;
using Obrazovashka.Models;
using Obrazovashka.Repositories.Interfaces;

namespace Obrazovashka.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly ApplicationDbContext _context;

        public CourseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddCourseAsync(Course course)
        {
            if (course != null)
            {
                await _context.Courses.AddAsync(course);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteCourseAsync(int courseId)
        {
            var course = await GetCourseByIdAsync(courseId);
            if (course != null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IList<Course>> GetAllCoursesAsync()
        {
            var courses = await _context.Courses.ToListAsync();
            if (courses != null)
                return courses;

            return null!;
        }

        public async Task<Course> GetCourseByIdAsync(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course != null)
                return course;

            return null!;
        }

        public async Task UpdateCourseAsync(Course course)
        {
            if (course != null)
            {
                _context.Courses.Update(course);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IList<Course>> SearchCoursesAsync(string searchTerm)
        {
            return await _context.Courses
                .Where(c => c.Title.Contains(searchTerm) || c.Description.Contains(searchTerm))
                .ToListAsync();
        }
    }
}