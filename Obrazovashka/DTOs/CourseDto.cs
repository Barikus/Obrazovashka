namespace Obrazovashka.DTOs
{
    public class CourseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string[] Tags { get; set; }
        public string ContentPath { get; set; }
        public int AuthorId { get; set; }
    }
}
