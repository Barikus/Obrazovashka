using Obrazovashka.Models;
using Obrazovashka.Repositories.Interfaces;

namespace Obrazovashka.Repositories
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        public Task AddEnrollmentAsync(Enrollment enrollment)
        {
            throw new NotImplementedException();
        }

        public Task<Enrollment> GetEnrollmentAsync(int userId, int courseId)
        {
            throw new NotImplementedException();
        }
    }
}
