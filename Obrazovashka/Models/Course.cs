using System.ComponentModel.DataAnnotations;

namespace Obrazovashka.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string[] Tags { get; set; }
        public string Content { get; set; }
        public int AuthorId { get; set; }
    }
}
