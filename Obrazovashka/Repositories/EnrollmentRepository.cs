using System.Threading.Tasks;
using Obrazovashka.Repositories.Interfaces;
using Obrazovashka.Models;

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
