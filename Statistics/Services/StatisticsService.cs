using Statistics.Models;
using Statistics.Data;
using Microsoft.EntityFrameworkCore;

public class StatisticsService
{
    private readonly ApplicationDbContext _context;

    public StatisticsService(ApplicationDbContext context)
    {
        _context = context;
    }

    public class EventMessage
    {
        public string EventType { get; set; }
        public int? UserId { get; set; }
        public int? CourseId { get; set; }
        public int? Rating { get; set; }
        public string Feedback { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public async Task ProcessMessageAsync(string message)
    {
        try
        {
            var eventData = System.Text.Json.JsonSerializer.Deserialize<EventMessage>(message);

            if (eventData == null)
            {
                Console.WriteLine("Сообщение пустое или имеет неправильный формат.");
                return;
            }

            switch (eventData.EventType)
            {
                case "UserRegistered":
                    await UpdateUserCountAsync();
                    break;

                case "CourseCreated":
                    await UpdateCourseCountAsync();
                    break;

                case "CourseCompleted":
                    if (eventData.UserId.HasValue && eventData.CourseId.HasValue)
                    {
                        await UpdateCourseCompletionAsync(eventData.UserId.Value, eventData.CourseId.Value);
                    }
                    break;

                case "FeedbackSubmitted":
                    if (eventData.CourseId.HasValue && eventData.Rating.HasValue)
                    {
                        await UpdateCourseFeedbackAsync(eventData.CourseId.Value, eventData.Rating.Value);
                    }
                    break;

                case "CourseViewed":
                    if (eventData.CourseId.HasValue)
                    {
                        await UpdateCourseViewsAsync(eventData.CourseId.Value);
                    }
                    break;

                case "UserLoggedIn":
                    if (eventData.UserId.HasValue)
                    {
                        await UpdateUserLoginAsync(eventData.UserId.Value);
                    }
                    break;

                default:
                    Console.WriteLine($"Неизвестный тип события: {eventData.EventType}");
                    break;
            }

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка обработки события: {ex.Message}");
        }
    }

    private async Task UpdateUserCountAsync()
    {
        var stats = await _context.GlobalStatistics.FirstOrDefaultAsync() ?? new GlobalStatistics();
        stats.TotalUsers++;
        _context.GlobalStatistics.Update(stats);
    }

    private async Task UpdateCourseCountAsync()
    {
        var stats = await _context.GlobalStatistics.FirstOrDefaultAsync() ?? new GlobalStatistics();
        stats.TotalCourses++;
        _context.GlobalStatistics.Update(stats);
    }

    private async Task UpdateCourseCompletionAsync(int userId, int courseId)
    {
        var stats = await _context.GlobalStatistics.FirstOrDefaultAsync() ?? new GlobalStatistics();
        stats.CompletedCourses++;

        var courseStats = await _context.CourseStatistics.FirstOrDefaultAsync(cs => cs.CourseId == courseId)
                          ?? new CourseStatistics { CourseId = courseId };
        courseStats.Completions++;
        _context.CourseStatistics.Update(courseStats);
    }

    private async Task UpdateCourseFeedbackAsync(int courseId, int rating)
    {
        var courseStats = await _context.CourseStatistics.FirstOrDefaultAsync(cs => cs.CourseId == courseId)
                          ?? new CourseStatistics { CourseId = courseId };

        courseStats.AverageRating = (courseStats.AverageRating * courseStats.RatingCount + rating) / (courseStats.RatingCount + 1);
        courseStats.RatingCount++;

        _context.CourseStatistics.Update(courseStats);
    }

    private async Task UpdateCourseViewsAsync(int courseId)
    {
        var courseStats = await _context.CourseStatistics.FirstOrDefaultAsync(cs => cs.CourseId == courseId)
                          ?? new CourseStatistics { CourseId = courseId };
        courseStats.Views++;
        _context.CourseStatistics.Update(courseStats);
    }

    private async Task UpdateUserLoginAsync(int userId)
    {
        var userStats = await _context.UserStatistics.FirstOrDefaultAsync(us => us.UserId == userId);

        if (userStats == null)
        {
            userStats = new UserStatistics
            {
                UserId = userId,
                LastLogin = DateTime.UtcNow
            };

            _context.UserStatistics.Add(userStats);
        }
        else
        {
            userStats.LastLogin = DateTime.UtcNow;
            _context.UserStatistics.Update(userStats);
        }

        await _context.SaveChangesAsync();
    }
}