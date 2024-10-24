﻿using Web.Application.Dto_s;
using Web.Application.Dto_s.PrivateChat;

namespace Web.Application.Interfaces.IServices
{
    public interface IPrivateChatService
    {
        Task<IEnumerable<ChatItemDto>?> GetChatsItemsAsync(int userId);

        Task CreateChatAsync(PrivateChatUsersDto model);

        Task<bool> IsUserExistInChatAsync(int userId, int privateChatId);

        Task<string?> GetOtherUserNameAsync(int userId, int privateChatId);

        Task<string?> GetOtherUserProfileImagePathAsync(int userId, int privateChatId);
    }
}