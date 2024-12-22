using Obrazovashka.AuthService.DTOs;
using Obrazovashka.AuthService.Results;

namespace Obrazovashka.AuthService.Services
{
    public interface IUserService
    {
        Task<RegistrationResult> RegisterUserAsync(UserRegistrationDto registrationDto);
        Task<LoginResult> LoginUserAsync(UserLoginDto loginDto);
        Task<UserResult> GetUserByIdAsync(int userId);
        Task<ProfileUpdateResult> UpdateProfileAsync(UserProfileDto profileDto);
    }
}
