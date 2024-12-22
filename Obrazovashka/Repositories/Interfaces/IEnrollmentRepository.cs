using Obrazovashka.Models;

namespace Obrazovashka.Repositories.Interfaces
{
    public interface IEnrollmentRepository
    {
        Task AddEnrollmentAsync(Enrollment enrollment);
        Task<IList<Enrollment>> GetEnrollmentsByUserIdAsync(int userId);
        Task<Enrollment> GetEnrollmentAsync(int userId, int courseId);
        Task UpdateEnrollmentAsync(Enrollment enrollment);
    }
}
