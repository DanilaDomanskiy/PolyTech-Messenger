using AutoMapper;
using Web.Application.DTO_s.Message;
using Web.Application.Interfaces;
using Web.Application.Interfaces.IServices;
using Web.Core.Entites;
using Web.Core.IRepositories;

namespace Web.Application.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IEncryptionService _encryptionService;
        private readonly IMapper _mapper;

        public MessageService(
            IMessageRepository messageRepository, 
            IMapper mapper, 
            IEncryptionService encryptionService)
        {
            _messageRepository = messageRepository;
            _mapper = mapper;
            _encryptionService = encryptionService;
        }

        public async Task SaveMessageAsync(SaveMessageDto saveMessageDTO)
        {
            var message = _mapper.Map<Message>(saveMessageDTO);
            message.Content = _encryptionService.Encrypt(message.Content);
            await _messageRepository.CreateAsync(message);
        }

        public async Task<IEnumerable<ReadMessageDto>> GetMessagesByChatIdAsync(int chatId, int userId)
        {
            var messages = await _messageRepository.GetMessagesByChatIdAsync(chatId);

            var readMessages = _mapper.Map<IEnumerable<ReadMessageDto>>(messages)
                .Select(message =>
                {
                    message.Content = _encryptionService.Decrypt(message.Content);
                    message.IsSender = message.SenderId == userId;
                    return message;
                });

            return readMessages;
        }
    }
}
