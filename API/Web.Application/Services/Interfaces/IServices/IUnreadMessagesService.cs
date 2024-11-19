namespace Web.Application.Services.Interfaces.IServices
{
    public interface IUnreadMessagesService
    {
        Task СlearGroupUnreadMessagesAsync(Guid userId, Guid groupId);

        Task СlearPrivateChatUnreadMessagesAsync(Guid userId, Guid privateChatId);
    }
}