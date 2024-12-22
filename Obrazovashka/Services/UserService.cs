using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Obrazovashka.DTOs;
using Obrazovashka.Models;
using Obrazovashka.Repositories.Interfaces;
using Obrazovashka.Results;
using System.Security.Claims;

namespace Obrazovashka.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IRabbitMqService _rabbitMqService;

        public UserService(IUserRepository userRepository, IConfiguration configuration, IRabbitMqService rabbitMqService)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _rabbitMqService = rabbitMqService;
        }

        public async Task<LoginResult> LoginUserAsync(UserLoginDto loginDto)
        {
            var userResult = await _userRepository.GetUserByEmailAsync(loginDto.Email!);
            if (userResult.Success == true || VerifyPasswordHash(loginDto.Password!, userResult?.User?.PasswordHash!))
            {
                var token = GenerateJwtToken(userResult?.User!);

                var message = new
                {
                    UserId = userResult!.User!.Id,
                    EventType = "UserLoggedIn",
                    Timestamp = DateTime.UtcNow
                };
                _rabbitMqService.PublishMessage("statistics", message);

                return new LoginResult { Token = token, Success = true };
            }

            return new LoginResult { Success = false, Message = "Неправильная почта или пароль" };
        }


        public async Task<RegistrationResult> RegisterUserAsync(UserRegistrationDto registrationDto)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(registrationDto.Email);
            if (existingUser.Success == true)
            {
                return new RegistrationResult { Success = false, Message = "Пользователь с таким email уже существует." };
            }

            var user = new User
            {
                Email = registrationDto.Email,
                Username = registrationDto.Username,
                PasswordHash = registrationDto.Password,
                Role = registrationDto.Role,
            };

            await _userRepository.AddUserAsync(user);

            var message = new
            {
                UserId = user.Id,
                EventType = "UserRegistered",
                Timestamp = DateTime.UtcNow
            };

            _rabbitMqService.PublishMessage("statistics", message);

            return new RegistrationResult { Success = true, Message = "Регистрация прошла успешно." };
        }


        private string HashPassword(string password)
        {
            using var hmac = new HMACSHA512();
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            var salt = Convert.ToBase64String(hmac.Key);
            var hash = Convert.ToBase64String(hashBytes);

            return $"{salt}:{hash}";
        }

        private bool VerifyPasswordHash(string password, string storedHash)
        {
            var parts = storedHash.Split(':');
            var storedSalt = Convert.FromBase64String(parts[0]);
            var storedPasswordHash = Convert.FromBase64String(parts[1]);

            using var hmac = new HMACSHA512(storedSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            return storedPasswordHash.SequenceEqual(computedHash);
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString() ?? string.Empty),
                new Claim(ClaimTypes.Name, user.Username ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Role ?? string.Empty)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Key"] ?? string.Empty));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var expiration = DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["DurationInMinutes"] ?? "720"));

            var token = new JwtSecurityToken(
                issuer: _configuration["Issuer"],
                audience: _configuration["Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<UserResult> GetUserByIdAsync(int userId)
        {
            var userResult = await _userRepository.GetUserByIdAsync(userId);
            if (userResult.Success == true)
                return userResult;

            return null!;
        }

        public async Task<ProfileUpdateResult> UpdateProfileAsync(UserProfileDto profileDto)
        {
            var userResult = await _userRepository.GetUserByIdAsync(profileDto.Id);

            if (userResult.Success == true && userResult.User != null)
            {
                userResult.User.Username = profileDto.Username;

                await _userRepository.UpdateUserAsync(userResult.User);

                return new ProfileUpdateResult { Success = true, Message = "Профиль успешно обновлён." };
            }

            return new ProfileUpdateResult { Success = false, Message = "Пользователь не найден." };
        }

    }
}
