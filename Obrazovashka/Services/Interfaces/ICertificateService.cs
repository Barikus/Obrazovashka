using Obrazovashka.Results;
using System.Threading.Tasks;

namespace Obrazovashka.Services
{
    public interface ICertificateService
    {
        Task<RegistrationResult> GenerateCertificateAsync(int userId, int courseId);
    }
}
