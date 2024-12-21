using Obrazovashka.Models;

namespace Obrazovashka.Results
{
    public class UserResult
    {
        public bool? Success { get; set; }
        public string? Message { get; set; }
        public User? User { get; set; }
    }
}
