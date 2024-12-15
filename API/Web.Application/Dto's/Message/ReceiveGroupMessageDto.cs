namespace Web.Application.Dto_s.Message
{
    public class ReceiveGroupMessageDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public Guid GroupId { get; set; }
        public Sender Sender { get; set; }
    }

    public class Sender
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ProfileImagePath { get; set; }
    }
}