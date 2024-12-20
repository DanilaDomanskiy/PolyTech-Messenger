﻿using AutoMapper;
using Web.Application.Dto_s.User;
using Web.Application.Services.Interfaces;
using Web.Application.Services.Interfaces.IServices;
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
            var user = await _userRepository.ReadByEmailAsync(userDTO.Login);

            if (user == null || !_passwordHasher.Verify(userDTO.Password, user.PasswordHash))
            {
                return null;
            }

            return _jwtProvider.GenerateToken(user);
        }

        public async Task<IEnumerable<SearchUserDto>?> SearchByEmailAsync(string email, Guid currentUserId)
        {
            var users = await _userRepository.ReadByEmailLettersAsync(email, currentUserId);
            return _mapper.Map<IEnumerable<SearchUserDto>>(users);
        }

        public async Task UpdateProfileImageAsync(Guid userId, string? filePath)
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

        public async Task<IEnumerable<SearchUserDto>?> GetNoGroupUsersAsync(string email, Guid groupId)
        {
            var users = await _userRepository.ReadNoGroupUsersAsync(email, groupId);
            return _mapper.Map<IEnumerable<SearchUserDto>>(users);
        }

        public async Task UpdateUserNameAsync(Guid userId, UserNameDto userNameDto)
        {
            var user = await _userRepository.ReadAsync(userId);

            if (user != null)
            {
                user.Name = userNameDto.Name;
                await _userRepository.UpdateAsync(user);
            }
        }

        public async Task UpdateUserPasswordAsync(Guid currentUserId, UserPasswordDto updatePasswordDto)
        {
            var user = await _userRepository.ReadAsync(currentUserId);

            if (user == null || !_passwordHasher.Verify(updatePasswordDto.OldPassword, user.PasswordHash))
            {
                throw new InvalidOperationException();
            }
            var passwordHash = _passwordHasher.Generate(updatePasswordDto.NewPassword);
            user.PasswordHash = passwordHash;
            await _userRepository.UpdateAsync(user);
        }
    }
}