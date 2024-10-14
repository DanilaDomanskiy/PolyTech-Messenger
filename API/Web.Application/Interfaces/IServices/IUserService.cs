using Web.Application.Dto_s.User;

namespace Web.Application.Interfaces.IServices
{
    public interface IUserService
    {
        Task UpdateProfileImageAsync(string filePath, int userId);

        Task<string?> GetUserNameAsync(int id);

        Task<string?> LoginUserAsync(AuthUserDto userDTO);

        Task RegisterUserAsync(RegisterUserDto userDTO);

        Task<IEnumerable<SearchUserDto>> SearchByEmailAsync(string email);
    }
}