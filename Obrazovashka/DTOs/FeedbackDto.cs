﻿namespace Obrazovashka.DTOs
{
    public class FeedbackDto
    {
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
    }
}
