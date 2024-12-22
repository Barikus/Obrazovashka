using Obrazovashka.Models;

namespace Obrazovashka.Results
{
    public class CourseCreateResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? CourseId { get; set; }
    }
}
