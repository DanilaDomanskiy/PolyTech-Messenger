namespace Web.Application.Dto_s.Message
{
    public class SendChatMessageDto
    {
        public Guid PrivateChatId { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
    }
}