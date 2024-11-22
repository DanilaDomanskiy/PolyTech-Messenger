using Web.Application.Dto_s;
using Web.Application.Dto_s.PrivateChat;

namespace Web.Application.Services.Interfaces.IServices
{
    public interface IPrivateChatService
    {
        Task<IEnumerable<PrivateChatItemDto>> GetChatsAsync(Guid userId);

        Task<Guid?> CreateChatAsync(PrivateChatUsersDto model);

        Task<bool> IsUserExistInChatAsync(Guid userId, Guid privateChatId);

        Task DeleteIfEmptyAsync(Guid chatId);

        Task<PrivateChatItemDto?> GetChatAsync(Guid chatId, Guid currentUserId);

        Task UpdateUnreadMessagesAsync(Guid privateChatId, Guid currentUserId);
    }
}