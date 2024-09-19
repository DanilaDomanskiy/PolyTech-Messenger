using CSharpFunctionalExtensions;
using Web.Application.DTO_s;
using Web.Application.DTO_s.User;

namespace Web.Application.Interfaces.IServices
{
    public interface IUserService
    {
        Task<UserDTO?> GetUserAsync(int id);
        Task<Result<string>> LoginUserAsync(AuthUserDTO userDTO);
        Task<Result> RegisterUserAsync(RegisterUserDTO userDTO);
    }
}