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

        public async Task<Guid> SaveMessageAsync(SaveMessageDto saveMessageDto)
        {
            var message = _mapper.Map<Message>(saveMessageDto);
            message.Content = _encryptionService.Encrypt(message.Content);
            return await _messageRepository.CreateAsync(message);
        }

        public async Task<IEnumerable<ReadGroupMessageDto>?> GetGroupMessagesAsync(Guid groupId, Guid userId, int page, int pageSize)
        {
            var messages = await _messageRepository.ReadGroupMessagesAsync(groupId, userId, page, pageSize);

            return messages?.Select(message =>
            {
                var readMessage = _mapper.Map<ReadGroupMessageDto>(message);
                readMessage.Content = _encryptionService.Decrypt(readMessage.Content);
                readMessage.IsSender = readMessage.SenderId == userId;
                readMessage.SenderName = message.Sender.Name;
                return readMessage;
            });
        }

        public async Task<IEnumerable<ReadChatMessageDto>?> GetChatMessagesAsync(Guid chatId, Guid userId, int page, int pageSize)
        {
            var messages = await _messageRepository.ReadChatMessagesAsync(chatId, userId, page, pageSize);

            return messages?.Select(message =>
            {
                var readMessage = _mapper.Map<ReadChatMessageDto>(message);
                readMessage.Content = _encryptionService.Decrypt(readMessage.Content);
                readMessage.IsSender = readMessage.SenderId == userId;
                return readMessage;
            });
        }

        public async Task DeleteAsync(Guid messageId, Guid userId)
        {
            var message = await _messageRepository.ReadAsync(messageId);
            if (message != null && message.SenderId == userId) await _messageRepository.DeleteAsync(message);
        }
    }
}