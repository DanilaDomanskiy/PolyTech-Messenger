namespace Web.Core.Entities
{
    public class Group : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? ImagePath { get; set; }
        public Guid CreatorId { get; set; }
        public User Creator { get; set; }
        public ICollection<User> Users { get; set; }
        public ICollection<Message> Messages { get; set; }
        public ICollection<UnreadMessages> UnreadMessages { get; set; }
    }
}