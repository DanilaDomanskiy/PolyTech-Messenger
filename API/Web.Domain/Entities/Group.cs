namespace Web.Core.Entites
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CreatorId { get; set; }
        public User Creator { get; set; }
        public ICollection<User> Users { get; set; }
        public ICollection<Message> Messages { get; set; }
    }
}
