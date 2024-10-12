using Web.Application.DTO_s.PrivateChat;

namespace Web.Application.Interfaces.IServices
{
    public interface IPrivateChatService
    {
        Task<IEnumerable<PrivateChatUserDto>> GetUserChatsAsync(int userId);

        Task<PrivateChatUsersDto?> GetChatUsersAsync(int userId);

        Task CreateChatAsync(PrivateChatUsersDto model);

        Task<bool> IsUserExistInChatAsync(int userId, int privateChatId);

        Task<string?> GetOtherUserNameAsync(int userId, int privateChatId);
    }
}