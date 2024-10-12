namespace Web.Application.Dto_s.User
{
    public class ProfileImageDto
    {
        public int UserId { get; set; }
        public byte[] Content { get; set; }
        public string Extension { get; set; }
    }
}