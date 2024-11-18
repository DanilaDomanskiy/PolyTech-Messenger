namespace Web.Application.Dto_s.Message
{
    public class ReadGroupMessageDto
    {
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public Guid SenderId { get; set; }
        public string SenderName { get; set; }
        public bool IsSender { get; set; }
    }
}