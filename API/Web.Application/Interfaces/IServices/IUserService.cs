using CSharpFunctionalExtensions;
using Web.Application.DTO_s;

namespace Web.Application.Interfaces.IServices
{
    public interface IUserService
    {
        Task<Result<string>> LoginUserAsync(AuthUserDTO userDTO);
        Task<Result> RegisterUserAsync(RegisterUserDTO userDTO);
    }
}