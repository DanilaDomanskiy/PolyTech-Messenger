namespace Web.Core.Entities
{
    public class UserConnection : IEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public string ConnectionId { get; set; }
    }
}