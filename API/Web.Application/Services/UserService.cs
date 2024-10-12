using AutoMapper;
using Web.Application.Dto_s.User;
using Web.Application.DTO_s.User;
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
        private readonly IFileStorageService _fileStorageService;

        public UserService(
            IPasswordHasher passwordHasher,
            IUserRepository userRepository,
            IJwtProvider jwtProvider,
            IMapper mapper,
            IFileStorageService fileStorageService)
        {
            _passwordHasher = passwordHasher;
            _userRepository = userRepository;
            _jwtProvider = jwtProvider;
            _mapper = mapper;
            _fileStorageService = fileStorageService;
        }

        public async Task RegisterUserAsync(RegisterUserDto userDTO)
        {
            var isUserExists = await _userRepository.IsUserExistsAsync(userDTO.Email);
            if (isUserExists)
            {
                throw new InvalidOperationException();
            }

            var user = _mapper.Map<User>(userDTO);
            user.ProfilePicturePath = "profile-image/default.jpg";
            user.PasswordHash = _passwordHasher.Generate(userDTO.Password);
            await _userRepository.CreateAsync(user);
        }

        public async Task<string?> LoginUserAsync(AuthUserDto userDTO)
        {
            var user = await _userRepository.ReadAsyncByEmail(userDTO.Login);

            if (user == null || !_passwordHasher.Verify(userDTO.Password, user.PasswordHash))
            {
                return null;
            }

            var token = _jwtProvider.GenerateToken(user);

            return token;
        }

        public async Task<string?> GetUserNameAsync(int id)
        {
            var user = await _userRepository.ReadAsync(id);
            return user?.Name ?? null;
        }

        public async Task<IEnumerable<SearchUserDto>> SearchByEmailAsync(string email)
        {
            var users = await _userRepository.ReadAsyncByEmailLetters(email);
            return _mapper.Map<IEnumerable<SearchUserDto>>(users);
        }

        public async Task UpdateProfileImageAsync(ProfileImageDto model)
        {
            var path = $"profile-images/{model.UserId}.{model.Extension}";
            await _fileStorageService.SaveFileAsync(model.Content, path);
            var user = await _userRepository.ReadAsync(model.UserId);
            user.ProfilePicturePath = path;
            await _userRepository.UpdateAsync(user);
        }
    }
}