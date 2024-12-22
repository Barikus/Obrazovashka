using Obrazovashka.Repositories.Interfaces;
using Obrazovashka.Results;
using Obrazovashka.Services.Interfaces;

namespace Obrazovashka.Services
{
    public class CertificateService : ICertificateService
    {
        private readonly IEnrollmentRepository _enrollmentRepository;

        public CertificateService(IEnrollmentRepository enrollmentRepository)
        {
            _enrollmentRepository = enrollmentRepository;
        }

        Task<CertificateResult> ICertificateService.GenerateCertificateAsync(int userId, int courseId)
        {
            throw new NotImplementedException();
        }
    }
}
