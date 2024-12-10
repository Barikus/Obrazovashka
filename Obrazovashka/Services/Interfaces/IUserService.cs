using System.Threading.Tasks;
using Obrazovashka.DTOs;
using Obrazovashka.Results;

namespace Obrazovashka.Services
{
    public interface IUserService
    {
        Task<RegistrationResult> RegisterUserAsync(UserRegistrationDto registrationDto);
        Task<LoginResult> LoginUserAsync(UserLoginDto loginDto);
        Task<ProfileUpdateResult> UpdateProfileAsync(UserProfileDto profileDto);
    }
}
