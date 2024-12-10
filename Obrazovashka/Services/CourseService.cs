using Obrazovashka.DTOs;
using Obrazovashka.Models;
using Obrazovashka.Repositories.Interfaces;
using Obrazovashka.Results;
using Obrazovashka.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Obrazovashka.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;

        public CourseService(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<CourseCreateResult> CreateCourseAsync(CourseCreateDto courseDto)
        {
            var course = new Course
            {
                Title = courseDto.Title,
                Description = courseDto.Description,
                Content = courseDto.Content,
            };

            await _courseRepository.AddCourseAsync(course);
            return new CourseCreateResult { Success = true, CourseId = course.Id };
        }

        public async Task<IEnumerable<CourseDto>> GetCoursesAsync()
        {
            var courses = await _courseRepository.GetAllCoursesAsync();
            var courseDtos = new List<CourseDto>();

            foreach (var course in courses)
            {
                courseDtos.Add(new CourseDto
                {
                    Id = course.Id,
                    Title = course.Title,
                    Description = course.Description,
                    Content = course.Content,
                });
            }

            return courseDtos;
        }

        public async Task<CourseDto> GetCourseByIdAsync(int id)
        {
            var course = await _courseRepository.GetCourseByIdAsync(id);
            if (course == null) return null;

            return new CourseDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                Content = course.Content,
            };
        }

        public async Task<CourseUpdateResult> UpdateCourseAsync(int id, CourseUpdateDto courseDto)
        {
            var course = await _courseRepository.GetCourseByIdAsync(id);
            if (course == null) return new CourseUpdateResult { Success = false, Message = "Course not found." };

            course.Title = courseDto.Title;
            course.Description = courseDto.Description;
            await _courseRepository.UpdateCourseAsync(course);

            return new CourseUpdateResult { Success = true };
        }

        public async Task<DeletionResult> DeleteCourseAsync(int id)
        {
            var course = await _courseRepository.GetCourseByIdAsync(id);
            if (course == null) return new DeletionResult { Success = false, Message = "Course not found." };

            await _courseRepository.DeleteCourseAsync(id);
            return new DeletionResult { Success = true };
        }
    }
}
