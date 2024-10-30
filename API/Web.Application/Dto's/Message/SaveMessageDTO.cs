namespace Web.Application.Dto_s.Message
{
    public class SaveMessageDto
    {
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public Guid SenderId { get; set; }
        public Guid? GroupId { get; set; }
        public Guid? PrivateChatId { get; set; }
    }
}