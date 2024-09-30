﻿using Web.Application.DTO_s;
using Web.Application.Interfaces;
using Web.Application.Interfaces.IServices;
using Web.Core.Entites;
using Web.Core.IRepositories;

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
        
        public async Task RegisterUserAsync(RegisterUserDto userDTO)
        {
            var isUserExists = await _userRepository.IsUserExistsAsync(userDTO.Email);
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

            if (user == null)
            {
                return null;
            }

            return user.Name;
        }
    }
}
