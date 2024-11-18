namespace Web.Application.Dto_s.Message
{
    public class SendChatMessageDto
    {
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public Guid PrivateChatId { get; set; }
    }
}