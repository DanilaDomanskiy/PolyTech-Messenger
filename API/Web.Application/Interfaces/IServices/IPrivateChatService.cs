using Web.Application.DTO_s.PrivateChat;

namespace Web.Application.Interfaces.IServices
{
    public interface IPrivateChatService
    {
        Task<IEnumerable<PrivateChatUserDTO>> GetUserChatsAsync(int userId);
        Task<PrivateChatUsersDTO?> GetChatUsersAsync(int userId);
    }
}