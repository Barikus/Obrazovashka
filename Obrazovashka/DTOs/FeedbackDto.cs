namespace Obrazovashka.DTOs
{
    public class FeedbackDto
    {
        public int UserId { get; set; }
        public string Feedback { get; set; } = string.Empty;
        public int Rating { get; set; } // Оценка от 1 до 5
    }
}
