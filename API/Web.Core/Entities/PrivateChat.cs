namespace Web.Core.Entities
{
    public class PrivateChat : IEntity
    {
        public Guid Id { get; set; }
        public ICollection<User> Users { get; set; }
        public ICollection<Message> Messages { get; set; }
        public ICollection<UnreadMessages> UnreadMessages { get; set; }
        public ICollection<UserConnection> ActiveUserConnections { get; set; }
    }
}