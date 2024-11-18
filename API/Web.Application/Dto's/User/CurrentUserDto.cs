namespace Web.Application.Dto_s.User
{
    public class CurrentUserDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? ProfileImagePath { get; set; }
    }
}