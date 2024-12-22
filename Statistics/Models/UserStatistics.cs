using System.ComponentModel.DataAnnotations;

namespace Statistics.Models
{
    public class UserStatistics
    {
        [Key]
        public int UserId { get; set; }
        public DateTime LastLogin { get; set; }
    }
}