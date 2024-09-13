namespace Web.Core.Entites
{
    public class Message
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
    }
}
