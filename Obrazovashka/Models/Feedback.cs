using System.ComponentModel.DataAnnotations;

namespace Obrazovashka.Models
{
    public class Feedback
    {
        [Key]
        public int? Id { get; set; }
        public int? EnrollmentId { get; set; }
        public string? Comment { get; set; }
        public int? Rating { get; set; }
    }
}
