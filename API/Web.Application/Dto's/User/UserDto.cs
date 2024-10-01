using Web.Core.Entites;

namespace Web.Application.Dto_s.User
{
    public class UserDto
    {
        public string Name { get; set; }
        public ICollection<PrivateChat> PrivateChatsAsUser1 { get; set; }
        public ICollection<PrivateChat> PrivateChatsAsUser2 { get; set; }
    }
}
