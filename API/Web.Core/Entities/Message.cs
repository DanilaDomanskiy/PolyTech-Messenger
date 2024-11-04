namespace Web.Core.Entities
{
    public class Message : IEntity
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public Guid SenderId { get; set; }
        public User Sender { get; set; }
        public Guid? GroupId { get; set; }
        public Group Group { get; set; }
        public Guid? PrivateChatId { get; set; }
        public PrivateChat PrivateChat { get; set; }
    }
}