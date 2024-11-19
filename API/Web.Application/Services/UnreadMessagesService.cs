using Web.Application.Services.Interfaces.IServices;
using Web.Core.IRepositories;

namespace Web.Application.Services
{
    public class UnreadMessagesService : IUnreadMessagesService
    {
        private readonly IUnreadMessagesRepository _unreadMessagesRepository;

        public UnreadMessagesService(IUnreadMessagesRepository unreadMessagesRepository)
        {
            _unreadMessagesRepository = unreadMessagesRepository;
        }

        public async Task СlearGroupUnreadMessagesAsync(Guid userId, Guid groupId)
        {
            await _unreadMessagesRepository.СlearGroupUnreadMessagesAsync(userId, groupId);
        }

        public async Task СlearPrivateChatUnreadMessagesAsync(Guid userId, Guid privateChatId)
        {
            await _unreadMessagesRepository.СlearPrivateChatUnreadMessagesAsync(userId, privateChatId);
        }
    }
}