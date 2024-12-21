namespace Obrazovashka.DTOs
{
    public class CourseUpdateDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string[]? Tags { get; set; }
        public IList<IFormFile>? ContentFiles { get; set; } = new List<IFormFile>();
    }
}
