namespace Web.Application.DTO_s.Message
{
    public class ReadMessageDto
    {
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public int SenderId { get; set; }
        public bool IsSender { get; set; }
    }
}