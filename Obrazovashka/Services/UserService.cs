using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
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

        public UserService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<LoginResult> LoginUserAsync(UserLoginDto loginDto)
        {
            var user = await _userRepository.GetUserByEmailAsync(loginDto.Email);
            if (user == null || !VerifyPasswordHash(loginDto.Password, user.PasswordHash))
            {
                return new LoginResult { Success = false, Message = "Неправильная почта или пароль" }; // Ошибка 401
            }

            var token = GenerateJwtToken(user);
            return new LoginResult { Token = token, Success = true };
        }


        public async Task<RegistrationResult> RegisterUserAsync(UserRegistrationDto registrationDto)
        {
            // Проверяем, существует ли пользователь с этой электронной почтой
            if (await _userRepository.GetUserByEmailAsync(registrationDto.Email) != null)
            {
                return new RegistrationResult { Success = false, Message = "Пользователь с такой электронной почтой уже существует!" };
            }

            // Создаём нового пользователя, если проверки пройдены
            var user = new User
            {
                Username = registrationDto.Username,
                Email = registrationDto.Email,
                PasswordHash = HashPassword(registrationDto.Password),
                Role = registrationDto.Role
            };

            await _userRepository.AddUserAsync(user);

            return new RegistrationResult { Success = true, Message = "Пользователь успешно зарегистрирован." };
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
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var expiration = DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["DurationInMinutes"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["Issuer"],
                audience: _configuration["Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public async Task<UserProfileDto> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) return null;

            return new UserProfileDto
            {
                Username = user.Username,
                Email = user.Email
            };
        }

        public async Task<ProfileUpdateResult> UpdateProfileAsync(UserProfileDto profileDto)
        {
            var user = await _userRepository.GetUserByEmailAsync(profileDto.Email);

            if (user == null)
            {
                return new ProfileUpdateResult { Success = false, Message = "Пользователь не найден." };
            }

            user.Username = profileDto.Username;
            user.Email = profileDto.Email;

            await _userRepository.UpdateUserAsync(user);

            return new ProfileUpdateResult { Success = true, Message = "Профиль успешно обновлён."};
        }

    }
}
