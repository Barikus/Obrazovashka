using System.ComponentModel.DataAnnotations;

namespace Statistics.Models
{
    public class GlobalStatistics
    {
        [Key]
        public int Id { get; set; } = 1;
        public int TotalUsers { get; set; } = 0;
        public int TotalCourses { get; set; } = 0;
        public int CompletedCourses { get; set; } = 0;
        public double AverageCourseRating { get; set; } = 0.0;
        public int TotalReviews { get; set; } = 0;
    }
}