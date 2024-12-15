using Web.Application.Dto_s.Message;

namespace Web.Application.Services.Interfaces.IServices
{
    public interface IMessageService
    {
        Task<IEnumerable<ReceiveGroupMessageDto>?> GetGroupMessagesAsync(Guid groupId, Guid userId, int page, int pageSize);

        Task<IEnumerable<ReceiveChatMessageDto>?> GetChatMessagesAsync(Guid chatId, Guid userId, int page, int pageSize);

        public Task<Guid> SaveMessageAsync(SaveMessageDto saveMessageDTO);

        Task DeleteAsync(Guid messageId, Guid userId);
    }
}