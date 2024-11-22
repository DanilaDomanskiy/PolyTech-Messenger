using Web.Application.Dto_s.Group;
using Web.Application.Dto_s.User;

namespace Web.Application.Services.Interfaces.IServices
{
    public interface IGroupService
    {
        Task AddUserAsync(GroupUserDto dto);
        Task ChangeGroupNameAsync(GroupNameDto groupNameDto);
        Task<Guid> CreateAsync(CreateGroupDto group);

        Task DeleteAsync(Guid groupId);
        Task DeleteUserAsync(GroupUserDto dto);
        Task<GroupItemDto?> GetGroupAsync(Guid groupId, Guid userId);
        Task<IEnumerable<GroupItemDto>> GetGroupsAsync(Guid userId);
        Task<IEnumerable<SearchUserDto>?> GetGroupUsersAsync(Guid groupId);
        Task<bool> IsUserExistInGroupAsync(Guid userId, Guid groupId);

        Task<bool> IsUserGroupCreatorAsync(Guid userId, Guid groupId);

        Task UpdateUnreadMessagesAsync(Guid groupId, Guid userId);
    }
}