using Web.Application.Dto_s.User;

namespace Web.Application.Services.Interfaces.IServices
{
    public interface IUserService
    {
        Task UpdateProfileImageAsync(Guid userId, string? filePath);

        Task<string?> LoginUserAsync(AuthUserDto userDto);

        Task AddUserAsync(RegisterUserDto userDto);

        Task<IEnumerable<SearchUserDto>?> SearchByEmailAsync(string email, Guid currentUserId);

        Task<CurrentUserDto?> GetUserAsync(Guid userId);

        Task<IEnumerable<SearchUserDto>?> GetNoGroupUsersAsync(string email, Guid groupId);

        Task UpdateUserNameAsync(Guid userId, UserNameDto userNameDto);

        Task UpdateUserPasswordAsync(Guid currentUserId, UserPasswordDto updatePasswordDto);
    }
}