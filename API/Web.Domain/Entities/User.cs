using Web.Core.Entities;

namespace Web.Core.Entites
{
    public class User : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public ICollection<Message> SentMessages { get; set; }
        public ICollection<Group> Groups { get; set; }
        public ICollection<PrivateChat> PrivateChatsAsUser1 { get; set; }
        public ICollection<PrivateChat> PrivateChatsAsUser2 { get; set; }
    }
}