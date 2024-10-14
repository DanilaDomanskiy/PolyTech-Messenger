namespace Web.Core.Entities
{
    public class MessageFile : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public int MessageId { get; set; }
        public Message Message { get; set; }
    }
}