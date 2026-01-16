using System;

namespace api.Models
{
    public class ErrorLog
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? StackTrace { get; set; }
        public string Source { get; set; } = "Unknown";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
