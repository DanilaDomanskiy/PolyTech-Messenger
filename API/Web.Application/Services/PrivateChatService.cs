using CSharpFunctionalExtensions;
using Web.Application.DTO_s.PrivateChat;
using Web.Application.Interfaces.IServices;
using Web.Core.Entites;
using Web.Core.IRepositories;

namespace Web.Application.Services
{
    public class PrivateChatService : IPrivateChatService
    {
        private readonly IPrivateChatRepository _privateChatRepository;

        public PrivateChatService(IPrivateChatRepository privateChatRepository)
        {
            _privateChatRepository = privateChatRepository;
        }

        public async Task<IEnumerable<PrivateChatUserDTO>> GetUserChatsAsync(int userId)
        {
            var chats = await _privateChatRepository.GetChatsAsync(userId);
            return chats.Select(chat =>
            {
                var otherUser = chat.User1Id == userId ? chat.User2 : chat.User1;

                return new PrivateChatUserDTO
                {
                    PrivateChatId = chat.Id,
                    UserName = otherUser.Name
                };
            });
        }
        
        public async Task<PrivateChatUsersDTO?> GetChatUsersAsync(int id)
        {
            var chat = await _privateChatRepository.GetChatAsync(id);

            if (chat == null)
            {
                return null;
            }

            return new PrivateChatUsersDTO
            { 
                User1Id = chat.User1Id,
                User2Id = chat.User2Id
            };
        }

        public async Task CreateChatAsync(PrivateChatUsersDTO model)
        {
            var privateChat = new PrivateChat
            {
                User1Id = model.User1Id,
                User2Id = model.User2Id
            };
            await _privateChatRepository.AddChatAsync(privateChat);
        }

        public async Task<bool> IsUserExistInChat(int userId, int privateChatId)
        {
            var privateChat = await _privateChatRepository.GetChatAsync(privateChatId);

            return privateChat == null ? false : 
                privateChat.User1Id == userId || 
                privateChat.User2Id == userId;
        }

        public async Task<string?> GetOtherUserNameAsync(int userId, int privateChatId)
        {
            var privateChat = await _privateChatRepository.GetChatAsync(privateChatId);
            return privateChat == null ? null 
                : privateChat.User1Id == userId 
                ? privateChat.User2.Name
                : privateChat.User2Id == userId
                ? privateChat.User1.Name 
                : null;
        }
    }
}
