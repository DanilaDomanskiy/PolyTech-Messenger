namespace Web.Application.Dto_s.Group
{
    public class GroupItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? ImagePath { get; set; }
        public bool IsCreator { get; set; }
        public int UnreadMessagesCount { get; set; }
        public LastMessage? LastMessage { get; set; }
    }

    public class LastMessage
    {
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public Sender Sender { get; set; }
    }

    public class Sender
    {
        public Guid SenderId { get; set; }
        public string SenderName { get; set; }
    }
}