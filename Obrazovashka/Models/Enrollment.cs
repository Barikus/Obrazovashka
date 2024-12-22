using System.ComponentModel.DataAnnotations;

namespace Obrazovashka.Models
{
    public class Enrollment
    {
        [Key]
        public int? Id { get; set; }
        public int? UserId { get; set; }
        public int? CourseId { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public bool Completed { get; set; } = false;
        public double? Rating { get; set; }
        public string? Feedback { get; set; } = string.Empty;
    }
}
