using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Obrazovashka.Repositories.Interfaces;
using Obrazovashka.Results;
using Obrazovashka.Services.Interfaces;

namespace Obrazovashka.Services
{
    public class CertificateService : ICertificateService
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICourseRepository _courseRepository;

        public CertificateService(IEnrollmentRepository enrollmentRepository, IUserRepository userRepository, ICourseRepository courseRepository)
        {
            _enrollmentRepository = enrollmentRepository;
            _userRepository = userRepository;
            _courseRepository = courseRepository;
        }

        public async Task<CertificateResult> GenerateCertificateAsync(int userId, int courseId)
        {
            // Проверяем, завершён ли курс
            var enrollment = await _enrollmentRepository.GetEnrollmentAsync(userId, courseId);
            if (enrollment == null || !(enrollment.Completed == true))
            {
                return new CertificateResult { Success = false, Message = "Курс не завершён. Сертификат недоступен." };
            }

            // Получаем информацию о пользователе и курсе
            var userResult = await _userRepository.GetUserByIdAsync(userId);
            if (!userResult.Success || userResult.User == null)
            {
                return new CertificateResult { Success = false, Message = "Пользователь не найден." };
            }

            var course = await _courseRepository.GetCourseByIdAsync(courseId);
            if (course == null)
            {
                return new CertificateResult { Success = false, Message = "Курс не найден." };
            }

            var userName = userResult.User.Username;
            var courseTitle = course.Title;

            // Путь к файлу сертификата
            var certificatePath = $"certificates/{userName?.Replace(" ", "_")}_certificate_for_{courseTitle?.Replace(" ", "_")}.pdf";
            Directory.CreateDirectory("certificates");

            // Генерация PDF сертификата
            using (var writer = new PdfWriter(certificatePath))
            {
                using (var pdf = new PdfDocument(writer))
                {
                    var document = new Document(pdf);

                    // Оформление сертификата
                    document.Add(new Paragraph("СЕРТИФИКАТ О ЗАВЕРШЕНИИ")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(24));

                    document.Add(new Paragraph("\nПодтверждается, что")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(16));

                    document.Add(new Paragraph(userName)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(20));

                    document.Add(new Paragraph($"успешно завершил курс \"{courseTitle}\"")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(16));

                    document.Add(new Paragraph($"Дата завершения: {DateTime.UtcNow:dd.MM.yyyy}")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(14));

                    document.Add(new Paragraph("\nПоздравляем!\n")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(16));
                }
            }

            return new CertificateResult { Success = true, CertificateUrl = certificatePath, Message = "Сертификат успешно сгенерирован." };
        }
    }
}