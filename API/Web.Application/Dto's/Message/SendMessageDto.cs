namespace Web.Application.Dto_s.Message
{
    public class SendMessageDto
    {
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public int? PrivateChatId { get; set; }
        public int? GroupId { get; set; }
    }
}