using Web.Application.Dto_s.Group;

namespace Web.Application.Services.Interfaces.IServices
{
    public interface IGroupService
    {
        Task AddUserAsync(AddUserToGroupDto dto);

        Task<Guid> CreateAsync(CreateGroupDto group);

        Task DeleteAsync(Guid groupId);
        Task<GroupItemDto?> GetGroupAsync(Guid groupId, Guid userId);
        Task<IEnumerable<GroupItemDto>> GetGroupsAsync(Guid userId);

        Task<bool> IsUserExistInGroupAsync(Guid userId, Guid groupId);

        Task<bool> IsUserGroupCreatorAsync(Guid userId, Guid groupId);

        Task UpdateUnreadMessagesAsync(Guid groupId, Guid userId);
    }
}