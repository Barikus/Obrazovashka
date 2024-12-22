using Obrazovashka.DTOs;
using Obrazovashka.Results;

namespace Obrazovashka.Services.Interfaces
{
    public interface IEnrollmentService
    {
        Task<EnrollmentResult> EnrollInCourseAsync(EnrollmentDto enrollmentDto);
        Task<FeedbackResult> LeaveFeedbackAsync(int courseId, FeedbackDto feedbackDto);
    }
}
