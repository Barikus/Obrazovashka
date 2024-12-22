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

        public EnrollmentService(IEnrollmentRepository enrollmentRepository)
        {
            _enrollmentRepository = enrollmentRepository;
        }


        public async Task<EnrollmentResult> EnrollInCourseAsync(EnrollmentDto enrollmentDto)
        {
            var enrollment = new Enrollment
            {
                UserId = enrollmentDto.UserId,
                CourseId = enrollmentDto.CourseId
            };

            await _enrollmentRepository.AddEnrollmentAsync(enrollment);
            return new EnrollmentResult { Success = true, Message = "Пользователь подписался." };
        }

        public Task<FeedbackResult> LeaveFeedbackAsync(int courseId, FeedbackDto feedbackDto)
        {
            throw new NotImplementedException();
        }
    }
}
