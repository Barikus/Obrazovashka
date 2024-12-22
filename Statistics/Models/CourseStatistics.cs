using System.ComponentModel.DataAnnotations;

namespace Statistics.Models
{
    public class CourseStatistics
    {
        [Key]
        public int CourseId { get; set; }
        public int Views { get; set; } = 0;
        public int Completions { get; set; } = 0;
        public double AverageRating { get; set; } = 0.0;
        public int RatingCount { get; set; } = 0;
    }
}