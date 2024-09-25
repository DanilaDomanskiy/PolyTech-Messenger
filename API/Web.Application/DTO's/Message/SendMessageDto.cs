namespace Web.Application.DTO_s.Message
{
    public class SendMessageDto
    {
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public string ConnectionId { get; set; }
        public int? GroupId { get; set; }
        public int? PrivateChatId { get; set; }
    }
}
