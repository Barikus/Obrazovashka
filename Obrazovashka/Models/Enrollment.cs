    using System.ComponentModel.DataAnnotations;

namespace Obrazovashka.Models
{
    public class Enrollment
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public int FeedbackId { get; set; }
    }
}
