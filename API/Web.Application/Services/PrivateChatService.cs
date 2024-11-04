using AutoMapper;
using Web.Application.Dto_s;
using Web.Application.Dto_s.PrivateChat;
using Web.Application.Interfaces.IServices;
using Web.Core.Entities;
using Web.Core.IRepositories;

namespace Web.Application.Services
{
    public class PrivateChatService : IPrivateChatService
    {
        private readonly IPrivateChatRepository _privateChatRepository;
        private readonly IMapper _mapper;

        public PrivateChatService(IPrivateChatRepository privateChatRepository, IMapper mapper)
        {
            _privateChatRepository = privateChatRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PrivateChatDto>?> GetChatsAsync(Guid userId)
        {
            var chats = await _privateChatRepository.GetChatsAsync(userId);

            var privateChatUser = chats?.Select(chat =>
            {
                var otherUser = chat.User1Id == userId ? chat.User2 : chat.User1;

                return new PrivateChatDto
                {
                    Id = chat.Id,
                    Name = otherUser.Name,
                    ProfilePicturePath = otherUser.ProfilePicturePath
                };
            });

            return privateChatUser;
        }

        public async Task<Guid> CreateChatAsync(PrivateChatUsersDto model)
        {
            var privateChat = _mapper.Map<PrivateChat>(model);
            var chatId = await _privateChatRepository.GetChatIdIfExistsAsync(model.User1Id, model.User2Id);
            return chatId ?? await _privateChatRepository.CreateAsync(privateChat);
        }

        public async Task<bool> IsUserExistInChatAsync(Guid userId, Guid privateChatId)
        {
            var privateChat = await _privateChatRepository.ReadAsync(privateChatId);
            return privateChat != null && (privateChat.User1Id == userId ||
                privateChat.User2Id == userId);
        }
    }
}