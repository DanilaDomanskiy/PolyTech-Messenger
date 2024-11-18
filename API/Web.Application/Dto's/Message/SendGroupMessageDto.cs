namespace Web.Application.Dto_s.Message
{
    public class SendGroupMessageDto
    {
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public Guid? PrivateChatId { get; set; }
        public Guid? GroupId { get; set; }
    }
}