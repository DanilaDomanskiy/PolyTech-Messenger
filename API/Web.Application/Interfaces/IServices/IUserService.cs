using Web.Application.Dto_s.User;
using Web.Application.DTO_s.User;

namespace Web.Application.Interfaces.IServices
{
    public interface IUserService
    {
        Task UpdateProfileImageAsync(ProfileImageDto model);

        Task<string?> GetUserNameAsync(int id);

        Task<string?> LoginUserAsync(AuthUserDto userDTO);

        Task RegisterUserAsync(RegisterUserDto userDTO);

        Task<IEnumerable<SearchUserDto>> SearchByEmailAsync(string email);
    }
}