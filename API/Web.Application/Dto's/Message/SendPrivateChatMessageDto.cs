namespace Web.Application.DTO_s.Message
{
    public class SendPrivateChatMessageDto
    {
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public int PrivateChatId { get; set; }
    }
}