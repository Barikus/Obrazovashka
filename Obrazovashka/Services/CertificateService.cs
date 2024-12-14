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

        Task<CertificateResult> ICertificateService.GenerateCertificateAsync(int userId, int courseId)
        {
            throw new NotImplementedException();
        }
    }
}
