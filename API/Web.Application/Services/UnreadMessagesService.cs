using Web.Application.Interfaces.IServices;
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

        public async Task СlearUnreadMessagesAsync(Guid userId, Guid? privateChatId = null, Guid? groupId = null)
        {
            await _unreadMessagesRepository.СlearUnreadMessagesAsync(userId, privateChatId, groupId);
        }
    }
}