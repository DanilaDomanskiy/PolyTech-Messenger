namespace Web.Application.Dto_s.Message
{
    public class ReceiveChatMessageDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public Guid SenderId { get; set; }
    }
}