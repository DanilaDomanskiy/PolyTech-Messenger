using AutoMapper;
using Web.Application.DTO_s.Message;
using Web.Application.DTO_s.PrivateChat;
using Web.Application.DTO_s.User;
using Web.Core.Entities;

namespace Web.Application
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<SaveMessageDto, Message>();

            CreateMap<Message, ReadMessageDto>()
                .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src => src.Sender.Name));

            CreateMap<RegisterUserDto, User>();

            CreateMap<PrivateChatUsersDto, PrivateChat>();

            CreateMap<PrivateChat, PrivateChatUsersDto>();

            CreateMap<User, SearchUserDto>();
        }
    }
}