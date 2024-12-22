using System.ComponentModel.DataAnnotations;

namespace Obrazovashka.Models
{
    public class User
    {
        [Key]
        public int? Id { get; set; }

        [Required]
        public string? Username { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? PasswordHash { get; set; }

        [Required]
        public string? Role { get; set; }
    }
}