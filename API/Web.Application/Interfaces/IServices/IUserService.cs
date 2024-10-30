using Web.Application.Dto_s.User;

namespace Web.Application.Interfaces.IServices
{
    public interface IUserService
    {
        Task UpdateProfileAsync(string filePath, Guid userId);

        Task<string?> GetUserNameAsync(Guid id);

        Task<string?> LoginUserAsync(AuthUserDto userDTO);

        Task RegisterUserAsync(RegisterUserDto userDTO);

        Task<IEnumerable<SearchUserDto>> SearchByEmailAsync(string email, Guid currentUserId);

        Task<CurrentUserDto?> GetUserAsync(Guid userId);
    }
}