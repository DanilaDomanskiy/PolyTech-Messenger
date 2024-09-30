namespace Web.Application.DTO_s.Message
{
    public class SaveMessageDto
    {
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public int SenderId { get; set; }
        public int? GroupId { get; set; }
        public int? PrivateChatId { get; set; }
    }
}
