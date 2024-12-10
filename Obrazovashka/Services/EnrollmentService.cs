using System.Threading.Tasks;
using Obrazovashka.DTOs;
using Obrazovashka.Results;

namespace Obrazovashka.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        public Task<EnrollmentResult> EnrollInCourseAsync(EnrollmentDto enrollmentDto)
        {
            throw new NotImplementedException();
        }

        public Task<FeedbackResult> LeaveFeedbackAsync(int courseId, FeedbackDto feedbackDto)
        {
            throw new NotImplementedException();
        }
    }
}
