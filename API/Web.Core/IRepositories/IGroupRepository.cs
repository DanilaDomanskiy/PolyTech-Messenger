using Web.Core.Entities;

namespace Web.Core.IRepositories
{
    public interface IGroupRepository : IBaseRepository<Group>
    {
        Task AddUserToGroupAsync(Guid userId, Guid groupId);

        Task<IEnumerable<Group>?> GetGroupsAsync(Guid userId);

        Task<bool> IsUserExistInGroupAsync(Guid userId, Guid groupId);

        Task<bool> IsUserGroupCreatorAsync(Guid userId, Guid groupId);
    }
}