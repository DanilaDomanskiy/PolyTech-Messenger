﻿using AutoMapper;
using Web.Application.Dto_s;
using Web.Application.Dto_s.PrivateChat;
using Web.Application.Interfaces.IServices;
using Web.Core.Entities;
using Web.Core.IRepositories;

namespace Web.Application.Services
{
    public class PrivateChatService : IPrivateChatService
    {
        private readonly IPrivateChatRepository _privateChatRepository;
        private readonly IMapper _mapper;

        public PrivateChatService(IPrivateChatRepository privateChatRepository, IMapper mapper)
        {
            _privateChatRepository = privateChatRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ChatItemDto>?> GetChatsItemsAsync(int userId)
        {
            var chats = await _privateChatRepository.GetChatsAsync(userId);

            var privateChatUser = chats.Select(chat =>
            {
                var otherUser = chat.User1Id == userId ? chat.User2 : chat.User1;

                return new ChatItemDto
                {
                    Id = chat.Id,
                    Name = otherUser.Name,
                    Image = otherUser.ProfilePicturePath
                };
            });

            return privateChatUser;
        }

        public async Task CreateChatAsync(PrivateChatUsersDto model)
        {
            var privateChat = _mapper.Map<PrivateChat>(model);
            await _privateChatRepository.CreateAsync(privateChat);
        }

        public async Task<bool> IsUserExistInChatAsync(int userId, int privateChatId)
        {
            var privateChat = await _privateChatRepository.ReadAsync(privateChatId);

            return privateChat != null && (privateChat.User1Id == userId ||
                privateChat.User2Id == userId);
        }

        public async Task<string?> GetOtherUserNameAsync(int userId, int privateChatId)
        {
            var privateChat = await _privateChatRepository.ReadAsync(privateChatId);

            if (privateChat == null)
            {
                return null;
            }

            if (privateChat.User1Id == userId)
            {
                return privateChat.User2.Name;
            }

            if (privateChat.User2Id == userId)
            {
                return privateChat.User1.Name;
            }

            return null;
        }

        public async Task<string?> GetOtherUserProfileImagePathAsync(int userId, int privateChatId)
        {
            var privateChat = await _privateChatRepository.ReadAsync(privateChatId);

            if (privateChat == null)
            {
                return null;
            }

            if (privateChat.User1Id == userId)
            {
                return privateChat.User2.ProfilePicturePath;
            }

            if (privateChat.User2Id == userId)
            {
                return privateChat.User1.ProfilePicturePath;
            }

            return null;
        }
    }
}