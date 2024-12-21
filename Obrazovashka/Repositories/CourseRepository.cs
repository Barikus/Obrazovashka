using System.Collections.Generic;
using System.Threading.Tasks;
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
            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();
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
            if (courses == null) return null!;

            return courses;
        }

        public async Task<Course> GetCourseByIdAsync(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null) return null!;
            
            return course;
        }

        public async Task UpdateCourseAsync(Course course)
        {
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
        }
    }
}