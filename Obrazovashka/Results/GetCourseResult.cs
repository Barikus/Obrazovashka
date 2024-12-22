using Obrazovashka.Models;

namespace Obrazovashka.Results
{
    public class GetCourseResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public Course? Course { get; set; }
    }
}
