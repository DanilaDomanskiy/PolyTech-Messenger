using Web.Application.Dto_s;
using Web.Application.Dto_s.PrivateChat;

namespace Web.Application.Interfaces.IServices
{
    public interface IPrivateChatService
    {
        Task<IEnumerable<PrivateChatDto>?> GetChatsAsync(Guid userId);

        Task<Guid> CreateChatAsync(PrivateChatUsersDto model);

        Task<bool> IsUserExistInChatAsync(Guid userId, Guid privateChatId);
    }
}