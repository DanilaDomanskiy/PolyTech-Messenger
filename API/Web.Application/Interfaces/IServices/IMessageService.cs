using Web.Application.Dto_s.Message;

namespace Web.Application.Interfaces.IServices
{
    public interface IMessageService
    {
        Task<IEnumerable<ReadMessageDto>?> GetMessagesAsync(Guid chatId, Guid userId, int page, int pageSize);

        Task SaveMessageAsync(SaveMessageDto saveMessageDTO);
    }
}