using Web.Application.DTO_s;

namespace Web.Application.Interfaces.IServices
{
    public interface IUserService
    {
        Task<string?> GetUserNameAsync(int id);
        Task<string?> LoginUserAsync(AuthUserDto userDTO);
        Task RegisterUserAsync(RegisterUserDto userDTO);
    }
}