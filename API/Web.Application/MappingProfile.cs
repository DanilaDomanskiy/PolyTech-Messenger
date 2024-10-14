using AutoMapper;
using Web.Application.Dto_s.User;
using Web.Application.DTO_s.Message;
using Web.Application.DTO_s.PrivateChat;
using Web.Core.Entities;

namespace Web.Application
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<SaveMessageDto, Message>();

            CreateMap<Message, ReadMessageDto>();

            CreateMap<RegisterUserDto, User>();

            CreateMap<PrivateChatUsersDto, PrivateChat>();

            CreateMap<User, SearchUserDto>();
        }
    }
}