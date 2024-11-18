namespace Web.Application.Interfaces.IServices
{
    public interface IUnreadMessagesService
    {
        Task СlearUnreadMessagesAsync(Guid userId, Guid? privateChatId = null, Guid? groupId = null);
    }
}