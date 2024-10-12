using Web.Application.DTO_s.Message;

namespace Web.Application.Interfaces.IServices
{
    public interface IMessageService
    {
        Task<IEnumerable<ReadMessageDto>> GetMessagesByChatIdAsync(int chatId, int userId);

        Task SaveMessageAsync(SaveMessageDto saveMessageDTO);
    }
}