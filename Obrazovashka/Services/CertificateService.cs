using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Obrazovashka.Models;
using Obrazovashka.Repositories.Interfaces;
using Obrazovashka.Results;

namespace Obrazovashka.Services
{
    public class CertificateService : ICertificateService
    {
        private readonly IEnrollmentRepository _enrollmentRepository;

        public CertificateService(IEnrollmentRepository enrollmentRepository)
        {
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<CertificateResult> GenerateCertificateAsync(int userId, int courseId)
        {
            var completion = await _enrollmentRepository.GetEnrollmentAsync(userId, courseId);

            if (completion == null) return new CertificateResult { Success = false };

            var certificateUrl = $"http://myapp.com/certificates/{userId}/{courseId}.pdf";

            var certificate = new Certificate
            {
                EnrollmentId = completion.Id,
                CertificateUrl = certificateUrl
            };
    
            return new CertificateResult { Success = true, CertificateUrl = certificateUrl };
        }

    }
}
