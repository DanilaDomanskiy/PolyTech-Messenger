using Web.Application.DTO_s.PrivateChat;

namespace Web.Application.Interfaces.IServices
{
    public interface IPrivateChatService
    {
        Task<IEnumerable<PrivateChatUserDTO>> GetUserChatsAsync(int userId);
        Task<PrivateChatUsersDTO?> GetChatUsersAsync(int userId);
        Task CreateChatAsync(PrivateChatUsersDTO model);
        Task<bool> IsUserExistInChat(int userId, int privateChatId);
        Task<string?> GetOtherUserNameAsync(int userId, int privateChatId);
    }
}