using Web.Application.DTO_s;
using Web.Application.DTO_s.User;
using Web.Application.Interfaces;
using Web.Application.Interfaces.IServices;
using Web.Core.Entites;
using Web.Persistence.Repositories;

namespace Web.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserRepository _userRepository;
        private readonly IJwtProvider _jwtProvider;

        public UserService(
            IPasswordHasher passwordHasher,
            IUserRepository userRepository,
            IJwtProvider jwtProvider)
        {
            _passwordHasher = passwordHasher;
            _userRepository = userRepository;
            _jwtProvider = jwtProvider;
        }
        
        public async Task RegisterUserAsync(RegisterUserDTO userDTO)
        {
            var isUserExists = await _userRepository.IsUserExists(userDTO.Email);
            if (isUserExists)
            {
                throw new InvalidOperationException();
            }
            var user = new User
            {
                Name = userDTO.Name,
                Email = userDTO.Email,
                PasswordHash = _passwordHasher.Generate(userDTO.Password)
            };
            await _userRepository.AddUserAsync(user);
        }

        public async Task<string?> LoginUserAsync(AuthUserDTO userDTO)
        {
            var user = await _userRepository.ReadAsyncByEmail(userDTO.Email);

            if (user == null || !_passwordHasher.Verify(userDTO.Password, user.PasswordHash))
            {
                return null;
            }

            var token = _jwtProvider.GenerateToken(user);

            return token;
        }

        public async Task<UserDTO?> GetUserAsync(int id)
        {
            var user = await _userRepository.ReadAsyncById(id);

            if (user == null)
            {
                return null;
            }

            return new UserDTO
            {
                Name = user.Name,
                Email = user.Email
            };
        }
    }
}
