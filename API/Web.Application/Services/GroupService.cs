using AutoMapper;
using Web.Application.Dto_s.Group;
using Web.Application.Interfaces.IServices;
using Web.Core.Entities;
using Web.Core.IRepositories;

namespace Web.Application.Services
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IMapper _mapper;

        public GroupService(IMapper mapper, IGroupRepository groupRepository)
        {
            _mapper = mapper;
            _groupRepository = groupRepository;
        }

        public async Task<Guid> CreateAsync(CreateGroupDto groupDto)
        {
            var group = _mapper.Map<Group>(groupDto);
            return await _groupRepository.CreateAsync(group);
        }

        public async Task AddUserAsync(AddUserToGroupDto dto)
        {
            await _groupRepository.AddUserToGroupAsync(dto.UserId, dto.GropId);
        }

        public async Task<bool> IsUserGroupCreatorAsync(Guid userId, Guid groupId)
        {
            return await _groupRepository.IsUserGroupCreatorAsync(userId, groupId);
        }

        public async Task<IEnumerable<GroupItemDto>?> GetGroupsAsync(Guid userId)
        {
            var groups = await _groupRepository.GetGroupsAsync(userId);

            return groups?.Select(group =>
            {
                var dto = _mapper.Map<GroupItemDto>(group);
                dto.IsCreator = group.CreatorId == userId;
                return dto;
            }).ToList();
        }

        public async Task<bool> IsUserExistInGroupAsync(Guid userId, Guid groupId)
        {
            return await _groupRepository.IsUserExistInGroupAsync(userId, groupId);
        }

        public async Task DeleteAsync(Guid groupId)
        {
            await _groupRepository.DeleteAsync(groupId);
        }
    }
}