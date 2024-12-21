using System.ComponentModel.DataAnnotations;

namespace Obrazovashka.Models
{
    public class Completion
    {
        [Key]
        public int? Id { get; set; }
        public int? EnrollmentId { get; set; }
        public string? PassedStage { get; set; }
    }
}
