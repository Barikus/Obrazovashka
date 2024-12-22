using Obrazovashka.Models;
using Obrazovashka.DTOs;
using Obrazovashka.Repositories.Interfaces;
using Obrazovashka.Services.Interfaces;
using Obrazovashka.Results;


namespace Obrazovashka.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IRabbitMqService _rabbitMqService;

        public EnrollmentService(IEnrollmentRepository enrollmentRepository, IRabbitMqService rabbitMqService)
        {
            _enrollmentRepository = enrollmentRepository;
            _rabbitMqService = rabbitMqService;
        }

        public async Task<EnrollmentResult> EnrollInCourseAsync(EnrollmentDto enrollmentDto)
        {
            try
            {
                if (enrollmentDto.CourseId == null || enrollmentDto.UserId == 0)
                {
                    return new EnrollmentResult
                    {
                        Success = false,
                        Message = "Недействительные данные для записи на курс."
                    };
                }

                var enrollment = new Enrollment
                {
                    UserId = enrollmentDto.UserId,
                    CourseId = enrollmentDto.CourseId.Value,
                    EnrollmentDate = DateTime.UtcNow
                };

                await _enrollmentRepository.AddEnrollmentAsync(enrollment);
                return new EnrollmentResult { Success = true, Message = "Пользователь успешно записан на курс." };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при записи на курс: {ex.Message}");
                return new EnrollmentResult { Success = false, Message = "Ошибка при записи на курс." };
            }
        }

        public async Task<EnrollmentResult> CompleteCourseAsync(int userId, int courseId)
        {
            var enrollment = await _enrollmentRepository.GetEnrollmentAsync(userId, courseId);
            if (enrollment == null)
            {
                return new EnrollmentResult { Success = false, Message = "Курс не найден для данного пользователя." };
            }

            if (enrollment.Completed)
            {
                return new EnrollmentResult { Success = false, Message = "Курс уже завершён." };
            }

            enrollment.Completed = true;
            await _enrollmentRepository.UpdateEnrollmentAsync(enrollment);

            return new EnrollmentResult { Success = true, Message = "Курс успешно завершён." };
        }

        public async Task<IList<Enrollment>> GetUserEnrollmentsAsync(int userId)
        {
            if (userId <= 0)
            {
                return new List<Enrollment>();
            }

            return await _enrollmentRepository.GetEnrollmentsByUserIdAsync(userId);
        }

        public async Task<FeedbackResult> LeaveFeedbackAsync(int courseId, FeedbackDto feedbackDto)
        {
            try
            {
                if (feedbackDto == null)
                {
                    return new FeedbackResult { Success = false, Message = "Некорректные данные для отзыва." };
                }

                var enrollment = await _enrollmentRepository.GetEnrollmentAsync(feedbackDto.UserId, courseId);
                if (enrollment == null)
                {
                    return new FeedbackResult
                    {
                        Success = false,
                        Message = "Пользователь не записан на этот курс."
                    };
                }

                enrollment.Rating = feedbackDto.Rating;
                enrollment.Feedback = feedbackDto.Feedback ?? "";
                await _enrollmentRepository.UpdateEnrollmentAsync(enrollment);

                return new FeedbackResult
                {
                    Success = true,
                    Message = "Ваш отзыв успешно добавлен."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении отзыва: {ex.Message}");
                return new FeedbackResult { Success = false, Message = "Ошибка при добавлении отзыва." };
            }
        }
    }
}
