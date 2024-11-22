namespace Web.Core.Entities
{
    public class User : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastActive { get; set; }
        public string? ProfileImagePath { get; set; }
        public ICollection<UserConnection> Connections { get; set; }
        public ICollection<Message> SentMessages { get; set; }
        public ICollection<PrivateChat> PrivateChats { get; set; }
        public ICollection<Group> Groups { get; set; }
        public ICollection<UnreadMessages> UnreadMessages { get; set; }
    }
}