using Obrazovashka.Results;
using System.Threading.Tasks;

namespace Obrazovashka.Services.Interfaces
{
    public interface ICertificateService
    {
        Task<CertificateResult> GenerateCertificateAsync(int userId, int courseId);
    }
}
