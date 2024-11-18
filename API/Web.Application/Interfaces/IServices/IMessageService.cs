using Web.Application.Dto_s.Message;

namespace Web.Application.Interfaces.IServices
{
    public interface IMessageService
    {
        Task<IEnumerable<ReadGroupMessageDto>?> GetGroupMessagesAsync(Guid groupId, Guid userId, int page, int pageSize);

        Task<IEnumerable<ReadChatMessageDto>?> GetChatMessagesAsync(Guid chatId, Guid userId, int page, int pageSize);

        public Task<Guid> SaveMessageAsync(SaveMessageDto saveMessageDTO);

        Task DeleteAsync(Guid messageId, Guid userId);
    }
}