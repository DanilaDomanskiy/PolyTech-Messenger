using AutoMapper;
using Web.Application.Dto_s.Message;
using Web.Application.Services.Interfaces;
using Web.Application.Services.Interfaces.IServices;
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

        public async Task<IEnumerable<ReceiveGroupMessageDto>?> GetGroupMessagesAsync(Guid groupId, Guid userId, int page, int pageSize)
        {
            var messages = await _messageRepository.ReadGroupMessagesAsync(groupId, userId, page, pageSize);

            return messages?.Select(message =>
            {
                return new ReceiveGroupMessageDto
                {
                    Id = message.Id,
                    Content = _encryptionService.Decrypt(message.Content),
                    Timestamp = message.Timestamp,
                    GroupId = groupId,
                    Sender = new Sender
                    {
                        Id = message.SenderId,
                        Name = message.Sender.Name,
                        ProfileImagePath = message.Sender.ProfileImagePath
                    }
                };
            });
        }

        public async Task<IEnumerable<ReceiveChatMessageDto>?> GetChatMessagesAsync(Guid chatId, Guid userId, int page, int pageSize)
        {
            var messages = await _messageRepository.ReadChatMessagesAsync(chatId, userId, page, pageSize);

            return messages?.Select(message =>
            {
                var readMessage = _mapper.Map<ReceiveChatMessageDto>(message);
                readMessage.Content = _encryptionService.Decrypt(readMessage.Content);
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