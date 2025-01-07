using Microsoft.AspNetCore.Mvc;
using Obrazovashka.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Obrazovashka.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertificatesController : ControllerBase
    {
        private readonly ICertificateService _certificateService;
        private readonly ILogger<CertificatesController> _logger;

        public CertificatesController(ICertificateService certificateService, ILogger<CertificatesController> logger)
        {
            _certificateService = certificateService;
            _logger = logger;
        }

        [HttpGet("{userId}/course/{courseId}")]
        public async Task<IActionResult> GenerateCertificate(int userId, int courseId)
        {
            _logger.LogInformation($"Запрос на генерацию сертификата для пользователя ID {userId} и курса ID {courseId}");
            var result = await _certificateService.GenerateCertificateAsync(userId, courseId);
            if (result.Success)
            {
                _logger.LogInformation($"Сертификат успешно сгенерирован для пользователя ID {userId} и курса ID {courseId}");
                return Ok(result);
            }

            _logger.LogWarning($"Не удалось сгенерировать сертификат для пользователя ID {userId} и курса ID {courseId}");
            return NotFound();
        }
    }
}
