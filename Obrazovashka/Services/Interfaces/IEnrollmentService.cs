using Obrazovashka.DTOs;
using Obrazovashka.Models;
using Obrazovashka.Results;

namespace Obrazovashka.Services.Interfaces
{
    public interface IEnrollmentService
    {
        Task<EnrollmentResult> EnrollInCourseAsync(EnrollmentDto enrollmentDto);
        Task<IList<Enrollment>> GetUserEnrollmentsAsync(int userId);
        Task<FeedbackResult> LeaveFeedbackAsync(int courseId, FeedbackDto feedbackDto);
    }
}
