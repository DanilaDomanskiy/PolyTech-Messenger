using Web.Application.Dto_s.User;

namespace Web.Application.Interfaces.IServices
{
    public interface IUserService
    {
        Task UpdateProfileAsync(string filePath, Guid userId);

        Task<string?> LoginUserAsync(AuthUserDto userDto);

        Task AddUserAsync(RegisterUserDto userDto);

        Task<IEnumerable<SearchUserDto>?> SearchByEmailAsync(string email, Guid currentUserId);

        Task<CurrentUserDto?> GetUserAsync(Guid userId);
    }
}