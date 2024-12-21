using Obrazovashka.DTOs;
using Obrazovashka.Models;
using Obrazovashka.Repositories.Interfaces;
using Obrazovashka.Results;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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

        // Создание нового курса
        public async Task<CourseCreateResult> CreateCourseAsync(CourseDto courseDto)
        {
            var course = new Course
            {
                Title = courseDto.Title,
                Description = courseDto.Description,
                Tags = courseDto.Tags,
                AuthorId = courseDto.AuthorId,
                ContentPath = courseDto.ContentPath
            };

            await _courseRepository.AddCourseAsync(course);

            return new CourseCreateResult { Success = true, CourseId = course.Id };
        }

        // Получение курса по ID
        public async Task<CourseDto> GetCourseByIdAsync(int courseId)
        {
            var course = await _courseRepository.GetCourseByIdAsync(courseId);
            if (course == null) return null!;

            return new CourseDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                ContentPath = course.ContentPath,
                Tags = course.Tags,
                AuthorId = course.AuthorId
            };
        }

        // Получение всех курсов
        public async Task<IList<CourseDto>> GetCoursesAsync()
        {
            var courses = await _courseRepository.GetAllCoursesAsync();
            return courses.Select(course => new CourseDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                ContentPath = course.ContentPath,
                AuthorId = course.AuthorId,
                Tags = course.Tags
            }).ToList();
        }

        // Получение курсов текущего пользователя по ID
        public async Task<IList<CourseDto>> GetCoursesByUserIdAsync(int userId)
        {
            var courses = await _courseRepository.GetAllCoursesAsync();
            return courses.Where(c => c.AuthorId == userId)
                          .Select(course => new CourseDto
                          {
                              Id = course.Id,
                              Title = course.Title,
                              Description = course.Description,
                              ContentPath = course.ContentPath,
                              AuthorId = course.AuthorId,
                              Tags = course.Tags
                          }).ToList();
        }

        // Сохранение файла по пути курса
        public async Task<string> SaveFileAsync(IFormFile file, string courseFolderPath)
        {
            int chapterNumber = GetNextFileNumber(courseFolderPath);
            var fileName = $"{chapterNumber:D2}.txt"; // Используем двухзначный формат
            var filePath = Path.Combine(courseFolderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return filePath;
        }

        // Получение следующего номера файла
        private int GetNextFileNumber(string courseFolderPath)
        {
            var existingFiles = Directory.GetFiles(courseFolderPath, "*.txt")
                .Select(f => Path.GetFileNameWithoutExtension(f))
                .Select(int.Parse)
                .ToList();

            return existingFiles.Count == 0 ? 1 : existingFiles.Max() + 1;
        }

        // Получение всех файлов курса
        public async Task<IList<string>> GetCourseFilesAsync(string courseFolderPath)
        {
            var directoryInfo = new DirectoryInfo(courseFolderPath);
            return directoryInfo.GetFiles("*.txt").Select(f => f.Name).ToList();
        }

        // Удаление файла по имени
        public async Task<DeletionResult> DeleteFileAsync(int courseId, string fileName)
        {
            var course = await _courseRepository.GetCourseByIdAsync(courseId);
            if (course == null)
                return new DeletionResult { Success = false, Message = "Курс не найден." };

            var filePath = Path.Combine(course.ContentPath!, fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return new DeletionResult { Success = true };
            }

            return new DeletionResult { Success = false, Message = "Файл не найден." };
        }       

        // Получение содержимого курса из текстовых файлов
        public async Task<IList<string>> GetCourseContentsAsync(string courseFolderPath)
        {
            var directoryInfo = new DirectoryInfo(courseFolderPath);
            var files = directoryInfo.GetFiles("*.txt").OrderBy(f => f.Name);

            var contents = new List<string>();
            foreach (var file in files)
            {
                contents.Add(await GetContentFromFileAsync(file.FullName));
            }

            return contents;
        }

        // Получение содержания конкретного файла
        public async Task<string> GetContentFromFileAsync(string filePath)
        {
            if (File.Exists(filePath))
            {
                var content = await File.ReadAllTextAsync(filePath);
                return ReplaceYouTubeLinksWithEmbedded(content);
            }
            return null!;
        }

        // Замена ссылок на YouTube на встроенные видео
        private string ReplaceYouTubeLinksWithEmbedded(string content)
        {
            const string youtubePattern = @"(?:https?:\/\/)?(?:www\.)?(?:youtube\.com\/(?:[^\/\n\s]+\/\S+\/|(?:v|e(?:mbed)?)\/|.*[?&]v=)|youtu\.be\/)([a-zA-Z0-9_-]{11})";
            var regex = new Regex(youtubePattern, RegexOptions.Compiled);
            return regex.Replace(content, match =>
            {
                var videoId = match.Groups[1].Value;
                return $@"<iframe width=""560"" height=""315"" src=""https://www.youtube.com/embed/{videoId}"" frameborder=""0"" allow=""accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"" allowfullscreen></iframe>";
            });
        }

        // Обновление курса
        public async Task<CourseUpdateResult> UpdateCourseAsync(int id, CourseUpdateDto courseUpdateDto)
        {
            var course = await _courseRepository.GetCourseByIdAsync(id);
            if (course == null)
                return new CourseUpdateResult { Success = false, Message = "Курс не найден." };

            // Обновляем поля курса
            course.Title = courseUpdateDto.Title;
            course.Description = courseUpdateDto.Description;
            course.Tags = courseUpdateDto.Tags;

            // Удаление существующих файлов, если требуется
            if (courseUpdateDto.ContentFiles != null && courseUpdateDto.ContentFiles.Count > 0)
            {
                foreach (var file in courseUpdateDto.ContentFiles)
                {
                    if (file.Length > 0)
                    {
                        var existingFilePath = Path.Combine(course.ContentPath!, file.FileName);
                        if (File.Exists(existingFilePath))
                        {
                            File.Delete(existingFilePath); // Удаляем существующий файл
                        }

                        await SaveFileAsync(file, course.ContentPath!); // Сохраняем новый файл
                    }
                }
            }

            await _courseRepository.UpdateCourseAsync(course);
            return new CourseUpdateResult { Success = true };
        }

        // Удаление курса по ID
        public async Task<DeletionResult> DeleteCourseAsync(int courseId)
        {
            var course = await _courseRepository.GetCourseByIdAsync(courseId);
            if (course == null) return new DeletionResult { Success = false, Message = "Курс не найден." };

            await _courseRepository.DeleteCourseAsync(courseId);
            return new DeletionResult { Success = true };
        }
    }
}
