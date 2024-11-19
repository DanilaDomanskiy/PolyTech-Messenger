namespace Web.Core.Entities
{
    public class UnreadMessages : IEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid? PrivateChatId { get; set; }
        public PrivateChat PrivateChat { get; set; }
        public Guid? GroupId { get; set; }
        public Group Group { get; set; }
        public int Count { get; set; }
    }
}