using System.ComponentModel.DataAnnotations;

namespace Obrazovashka.Models
{
    public class Review
    {
        [Key]
        public int? Id { get; set; }
        public int EnrollmentId { get; set; }
        public string? ContentPath { get; set; }
        public int? Rating { get; set; }
        public int? DatePosted { get; set; }
    }
}
