using System.Threading.Tasks;
using Obrazovashka.Results;

namespace Obrazovashka.Services
{
    public class CertificateService : ICertificateService
    {
        public Task<RegistrationResult> GenerateCertificateAsync(int userId, int courseId)
        {
            throw new NotImplementedException();
        }
    }
}
