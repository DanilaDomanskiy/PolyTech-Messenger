using AutoMapper;
using Web.Application.Dto_s.Group;
using Web.Application.Dto_s.User;
using Web.Application.Services.Interfaces;
using Web.Application.Services.Interfaces.IServices;
using Web.Core.Entities;
using Web.Core.IRepositories;

namespace Web.Application.Services
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IMapper _mapper;
        private readonly IEncryptionService _encryptionService;

        public GroupService(IMapper mapper, IGroupRepository groupRepository, IEncryptionService encryptionService)
        {
            _mapper = mapper;
            _groupRepository = groupRepository;
            _encryptionService = encryptionService;
        }

        public async Task<Guid> CreateAsync(Guid creatorId, CreateGroupDto groupDto)
        {
            var group = _mapper.Map<Group>(groupDto);
            group.CreatorId = creatorId;
            return await _groupRepository.CreateAsync(group);
        }

        public async Task AddUserAsync(GroupUserDto dto)
        {
            await _groupRepository.AddUserToGroupAsync(dto.UserId, dto.GropId);
        }

        public async Task DeleteUserAsync(GroupUserDto dto)
        {
            await _groupRepository.DeleteUserFromGroupAsync(dto.UserId, dto.GropId);
        }

        public async Task<bool> IsUserGroupCreatorAsync(Guid userId, Guid groupId)
        {
            return await _groupRepository.IsUserGroupCreatorAsync(userId, groupId);
        }

        public async Task<IEnumerable<GroupItemDto>> GetGroupsAsync(Guid userId)
        {
            var groups = await _groupRepository.ReadGroupsAsync(userId) ?? Enumerable.Empty<Group>();

            return groups.Select(group =>
            {
                var lastMessage = group.Messages.FirstOrDefault();
                if (lastMessage != null)
                {
                    lastMessage.Content = _encryptionService.Decrypt(lastMessage.Content);
                }

                return new GroupItemDto
                {
                    Id = group.Id,
                    Name = group.Name,
                    ImagePath = group.ImagePath,
                    IsCreator = group.CreatorId == userId,
                    UnreadMessagesCount = group.UnreadMessages.Any() ? group.UnreadMessages.First().Count : 0,
                    LastMessage = lastMessage == null ? null : new Dto_s.Group.LastMessage
                    {
                        Content = lastMessage.Content,
                        Timestamp = lastMessage.Timestamp,
                        Sender = new Sender
                        {
                            SenderId = lastMessage.Sender.Id,
                            SenderName = lastMessage.Sender.Name
                        }
                    }
                };
            }).ToList();
        }

        public async Task<GroupItemDto?> GetGroupAsync(Guid groupId, Guid userId)
        {
            var group = await _groupRepository.ReadGroupAsync(groupId, userId);

            if (group == null) return null;

            var lastMessage = group.Messages.FirstOrDefault();

            if (lastMessage != null)
            {
                lastMessage.Content = _encryptionService.Decrypt(lastMessage.Content);
            }

            return new GroupItemDto
            {
                Id = group.Id,
                Name = group.Name,
                ImagePath = group.ImagePath,
                IsCreator = group.CreatorId == userId,
                UnreadMessagesCount = group.UnreadMessages.Any() ? group.UnreadMessages.First().Count : 0,
                LastMessage = lastMessage == null ? null : new Dto_s.Group.LastMessage
                {
                    Content = lastMessage.Content,
                    Timestamp = lastMessage.Timestamp,
                    Sender = new Sender
                    {
                        SenderId = lastMessage.Sender.Id,
                        SenderName = lastMessage.Sender.Name
                    }
                }
            };
        }

        public async Task<bool> IsUserExistInGroupAsync(Guid userId, Guid groupId)
        {
            return await _groupRepository.IsUserExistInGroupAsync(userId, groupId);
        }

        public async Task DeleteAsync(Guid groupId)
        {
            await _groupRepository.DeleteAsync(groupId);
        }

        public async Task UpdateUnreadMessagesAsync(Guid groupId, Guid userId)
        {
            await _groupRepository.UpdateUnreadMessagesAsync(groupId, userId);
        }

        public async Task<IEnumerable<SearchUserDto>?> GetGroupUsersAsync(Guid groupId)
        {
            var users = await _groupRepository.ReadGroupUsersAsync(groupId);
            return _mapper.Map<IEnumerable<SearchUserDto>>(users);
        }

        public async Task UpdateGroupNameAsync(GroupNameDto groupNameDto)
        {
            var group = await _groupRepository.ReadAsync(groupNameDto.GroupId);

            if (group != null)
            {
                group.Name = groupNameDto.Name;
                await _groupRepository.UpdateAsync(group);
            }
        }

        public async Task UpdateGroupImageAsync(Guid groupId, string? filePath)
        {
            var group = await _groupRepository.ReadAsync(groupId);

            if (group != null)
            {
                group.ImagePath = filePath;
                await _groupRepository.UpdateAsync(group);
            }
        }
    }
}