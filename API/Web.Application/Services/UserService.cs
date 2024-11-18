using AutoMapper;
using Web.Application.Dto_s.User;
using Web.Application.Interfaces;
using Web.Application.Interfaces.IServices;
using Web.Core.Entities;
using Web.Core.IRepositories;

namespace Web.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserRepository _userRepository;
        private readonly IJwtProvider _jwtProvider;
        private readonly IMapper _mapper;

        public UserService(
            IPasswordHasher passwordHasher,
            IUserRepository userRepository,
            IJwtProvider jwtProvider,
            IMapper mapper)
        {
            _passwordHasher = passwordHasher;
            _userRepository = userRepository;
            _jwtProvider = jwtProvider;
            _mapper = mapper;
        }

        public async Task AddUserAsync(RegisterUserDto userDto)
        {
            var isUserExists = await _userRepository.IsUserExistsAsync(userDto.Email);
            if (isUserExists)
            {
                throw new InvalidOperationException();
            }
            var user = _mapper.Map<User>(userDto);
            user.PasswordHash = _passwordHasher.Generate(userDto.Password);
            await _userRepository.CreateAsync(user);
        }

        public async Task<string?> LoginUserAsync(AuthUserDto userDTO)
        {
            var user = await _userRepository.ReadAsyncByEmail(userDTO.Login);

            if (user == null || !_passwordHasher.Verify(userDTO.Password, user.PasswordHash))
            {
                return null;
            }

            return _jwtProvider.GenerateToken(user);
        }

        public async Task<IEnumerable<SearchUserDto>?> SearchByEmailAsync(string email, Guid currentUserId)
        {
            var users = await _userRepository.ReadAsyncByEmailLetters(email, currentUserId);
            return _mapper.Map<IEnumerable<SearchUserDto>>(users);
        }

        public async Task UpdateProfileAsync(string filePath, Guid userId)
        {
            var user = await _userRepository.ReadAsync(userId);
            if (user != null)
            {
                user.ProfileImagePath = filePath;
                await _userRepository.UpdateAsync(user);
            }
        }

        public async Task<CurrentUserDto?> GetUserAsync(Guid userId)
        {
            var user = await _userRepository.ReadAsync(userId);
            return _mapper.Map<CurrentUserDto>(user);
        }
    }
}