using AutoMapper;
using Web.Application.Dto_s.PrivateChat;
using Web.Application.Services.Interfaces;
using Web.Application.Services.Interfaces.IServices;
using Web.Core.Entities;
using Web.Core.IRepositories;

namespace Web.Application.Services
{
    public class PrivateChatService : IPrivateChatService
    {
        private readonly IPrivateChatRepository _privateChatRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IEncryptionService _encryptionService;

        public PrivateChatService(
            IPrivateChatRepository privateChatRepository,
            IMapper mapper,
            IUserRepository userRepository,
            IEncryptionService encryptionService)
        {
            _privateChatRepository = privateChatRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _encryptionService = encryptionService;
        }

        public async Task<IEnumerable<PrivateChatItemDto>> GetChatsAsync(Guid currentUserId)
        {
            var chats = await _privateChatRepository.ReadChatsWithSecondUserAsync(currentUserId);

            if (chats == null) return Enumerable.Empty<PrivateChatItemDto>();

            return chats.Select(chat =>
            {
                var otherUser = chat.Users.FirstOrDefault(user => user.Id != currentUserId);

                var lastMessage = chat.Messages.FirstOrDefault();

                if (lastMessage != null && lastMessage.Content.Length % 4 == 0)
                {
                    lastMessage.Content = _encryptionService.Decrypt(lastMessage.Content);
                }

                return new PrivateChatItemDto
                {
                    Id = chat.Id,
                    SecondUser = otherUser != null ? _mapper.Map<SecondUser>(otherUser) : null,
                    LastMessage = lastMessage != null ? _mapper.Map<LastMessage>(lastMessage) : null,
                    UnreadMessagesCount = chat.UnreadMessages.Any() ? chat.UnreadMessages.First().Count : 0,
                };
            });
        }

        public async Task<Guid?> CreateChatAsync(Guid currentUserId, CreateChatDto model)
        {
            var chatId = await _privateChatRepository.ReadChatIdIfChatExistsAsync(currentUserId, model.OtherUserId);
            if (chatId.HasValue)
            {
                return chatId.Value;
            }

            var newChat = new PrivateChat
            {
                Users = new List<User>
                {
                    await _userRepository.ReadAsync(currentUserId),
                    await _userRepository.ReadAsync(model.OtherUserId)
                },
                Messages = new List<Message>()
            };

            await _privateChatRepository.CreateAsync(newChat);
            return newChat.Id;
        }

        public async Task<bool> IsUserExistInChatAsync(Guid userId, Guid privateChatId)
        {
            return await _privateChatRepository.IsUserExistInChatAsync(userId, privateChatId);
        }

        public async Task DeleteIfEmptyAsync(Guid chatId)
        {
            await _privateChatRepository.DeleteIfEmptyAsync(chatId);
        }

        public async Task<PrivateChatItemDto?> GetChatAsync(Guid chatId, Guid currentUserId)
        {
            var chat = await _privateChatRepository.ReadChatWithSecondUserAsync(chatId, currentUserId);

            if (chat == null) return null;

            var otherUser = chat.Users.FirstOrDefault(user => user.Id != currentUserId);
            var lastMessage = chat.Messages.LastOrDefault();

            if (lastMessage != null)
            {
                lastMessage.Content = _encryptionService.Decrypt(lastMessage.Content);
            }

            return new PrivateChatItemDto
            {
                Id = chat.Id,
                SecondUser = otherUser != null ? _mapper.Map<SecondUser>(otherUser) : null,
                LastMessage = lastMessage != null ? _mapper.Map<LastMessage>(lastMessage) : null,
                UnreadMessagesCount = chat.UnreadMessages.Any() ? chat.UnreadMessages.First().Count : 0,
            };
        }

        public async Task UpdateUnreadMessagesAsync(Guid privateChatId, Guid currentUserId)
        {
            await _privateChatRepository.UpdateUnreadMessagesAsync(privateChatId, currentUserId);
        }
    }
}