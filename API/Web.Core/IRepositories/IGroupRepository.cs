using Web.Core.Entities;

namespace Web.Core.IRepositories
{
    public interface IGroupRepository : IBaseRepository<Group>
    {
        Task AddUserToGroupAsync(Guid userId, Guid groupId);

        Task<IEnumerable<Group>?> ReadGroupsAsync(Guid userId);

        Task<bool> IsUserExistInGroupAsync(Guid userId, Guid groupId);

        Task<bool> IsUserGroupCreatorAsync(Guid userId, Guid groupId);

        Task UpdateUnreadMessagesAsync(Guid groupId, Guid userId);

        Task<Group?> ReadGroupAsync(Guid groupId, Guid userId);

        Task DeleteUserFromGroupAsync(Guid userId, Guid groupId);

        Task<IEnumerable<User>?> ReadGroupUsersAsync(Guid groupId);
    }
}