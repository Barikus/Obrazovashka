using Obrazovashka.AuthService.Models;

namespace Obrazovashka.Results
{
    public class CourseCreateResult
    {
        public bool? Success { get; set; }
        public string? Message { get; set; }
        public int? CourseId { get; set; }
    }
}
