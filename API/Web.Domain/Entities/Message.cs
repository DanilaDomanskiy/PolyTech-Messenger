namespace Web.Core.Entities
{
    public class Message : IEntity
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public int SenderId { get; set; }
        public User Sender { get; set; }
        public int? GroupId { get; set; }
        public Group Group { get; set; }
        public int? PrivateChatId { get; set; }
        public PrivateChat PrivateChat { get; set; }
        public int? FileId { get; set; }
        public MessageFile File { get; set; }
    }
}