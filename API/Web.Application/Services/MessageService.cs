using Web.Application.DTO_s.Message;
using Web.Application.Interfaces.IServices;
using Web.Core.Entites;
using Web.Persistence.Repositories;

namespace Web.Application.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;

        public MessageService(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task SaveMessageAsync(SaveMessageDto saveMessageDTO)
        {
            var message = new Message
            {
                Content = saveMessageDTO.Content,
                Timestamp = saveMessageDTO.Timestamp,
                SenderId = saveMessageDTO.SenderId, 
                GroupId = saveMessageDTO?.GroupId,
                PrivateChatId = saveMessageDTO?.PrivateChatId
            };

            await _messageRepository.SaveMessageAsync(message);
        }

        public async Task<IEnumerable<ReadMessageDto>> GetMessagesByChatIdAsync(int chatId, int userId)
        {
            var messages = await _messageRepository.GetMessagesByChatIdAsync(chatId);
            return messages.Select(message => new ReadMessageDto
            {
                Content = message.Content,
                Timestamp = message.Timestamp,
                SenderName = message.Sender.Name,
                SenderId = message.Sender.Id,
                IsSender = message.SenderId == userId
            });
        }
    }
}
