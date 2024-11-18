namespace Web.Application.Dto_s
{
    public class PrivateChatItemDto
    {
        public Guid Id { get; set; }
        public SecondUser SecondUser { get; set; }
        public LastMessage? LastMessage { get; set; }
        public int UnreadMessagesCount { get; set; }
    }

    public class LastMessage
    {
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public Guid SenderId { get; set; }
    }

    public class SecondUser
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? ProfileImagePath { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastActive { get; set; }
    }
}