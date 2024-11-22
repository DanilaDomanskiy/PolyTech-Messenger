namespace Web.Application.Dto_s.Message
{
    public class SendGroupMessageDto
    {
        public Guid GroupId { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
    }
}