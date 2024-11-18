namespace Web.Application.Dto_s.Group
{
    public class GroupItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? ImagePath { get; set; }
        public bool IsCreator { get; set; }
    }
}