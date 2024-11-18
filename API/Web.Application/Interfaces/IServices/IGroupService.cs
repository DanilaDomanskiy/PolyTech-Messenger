using Web.Application.Dto_s.Group;

namespace Web.Application.Interfaces.IServices
{
    public interface IGroupService
    {
        Task AddUserAsync(AddUserToGroupDto dto);

        Task<Guid> CreateAsync(CreateGroupDto group);

        Task DeleteAsync(Guid groupId);

        Task<IEnumerable<GroupItemDto>?> GetGroupsAsync(Guid userId);

        Task<bool> IsUserExistInGroupAsync(Guid userId, Guid groupId);

        Task<bool> IsUserGroupCreatorAsync(Guid userId, Guid groupId);
    }
}