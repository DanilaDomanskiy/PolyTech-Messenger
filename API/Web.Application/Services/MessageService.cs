using AutoMapper;
using Web.Application.Dto_s.Message;
using Web.Application.Interfaces;
using Web.Application.Interfaces.IServices;
using Web.Core.Entities;
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

        public async Task<IEnumerable<ReadMessageDto>?> GetMessagesAsync(Guid chatId, Guid userId, int page, int pageSize)
        {
            var messages = await _messageRepository.GetMessagesAsync(chatId, page, pageSize);

            var readMessages = messages.Select(message =>
            {
                var readMessage = _mapper.Map<ReadMessageDto>(message);
                readMessage.Content = _encryptionService.Decrypt(readMessage.Content);
                readMessage.IsSender = readMessage.SenderId == userId;
                return readMessage;
            });

            return readMessages;
        }
    }
}