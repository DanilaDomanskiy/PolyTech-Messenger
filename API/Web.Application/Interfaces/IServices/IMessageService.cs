using Web.Application.DTO_s.Message;

namespace Web.Application.Interfaces.IServices
{
    public interface IMessageService
    {
        Task<IEnumerable<ReadMessageDTO>> GetMessagesByChatIdAsync(int chatId);
        Task SaveMessageAsync(SaveMessageDTO saveMessageDTO);
    }
}