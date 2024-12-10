using System.ComponentModel.DataAnnotations;

namespace Obrazovashka.Models
{
    public class Certificate
    {
        [Key]
        public int Id { get; set; }
        public int EnrollmentId { get; set; }
        public string CertificateUrl { get; set; }
    }
}
