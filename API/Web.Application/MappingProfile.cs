using AutoMapper;
using Web.Application.Dto_s;
using Web.Application.Dto_s.Group;
using Web.Application.Dto_s.Message;
using Web.Application.Dto_s.User;
using Web.Core.Entities;

namespace Web.Application
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<SaveMessageDto, Message>();
            CreateMap<Message, ReadChatMessageDto>();
            CreateMap<Message, ReadGroupMessageDto>();
            CreateMap<RegisterUserDto, User>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.LastActive, opt => opt.MapFrom(src => DateTime.UtcNow));
            CreateMap<User, SearchUserDto>();
            CreateMap<User, CurrentUserDto>();
            CreateMap<CreateGroupDto, Group>();
            CreateMap<User, SecondUser>();
            CreateMap<Message, LastMessage>();
            CreateMap<Group, GroupItemDto>();
        }
    }
}