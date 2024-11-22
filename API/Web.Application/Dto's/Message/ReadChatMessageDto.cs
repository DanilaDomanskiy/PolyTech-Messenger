namespace Web.Application.Dto_s.Message
{
    public class ReadChatMessageDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public Guid SenderId { get; set; }
        public bool IsSender { get; set; }
    }
}