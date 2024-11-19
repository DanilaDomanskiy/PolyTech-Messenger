namespace Web.Application.Dto_s.User
{
    public class SearchUserDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string? ProfilePicturePath { get; set; }
        public bool IsActive { get; set; }
    }
}