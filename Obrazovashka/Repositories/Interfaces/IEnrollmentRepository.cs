using Obrazovashka.Models;

namespace Obrazovashka.Repositories.Interfaces
{
    public interface IEnrollmentRepository
    {
        Task AddEnrollmentAsync(Enrollment enrollment);
        Task<Enrollment> GetEnrollmentAsync(int userId, int courseId);
    }
}
