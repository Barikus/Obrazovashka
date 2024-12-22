using Obrazovashka.AuthService.Models;
using Obrazovashka.AuthService.Results;

namespace Obrazovashka.AuthService.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task AddUserAsync(User user);
        Task<UserResult> GetUserByEmailAsync(string email);
        Task<UserResult> GetUserByIdAsync(int userId);
        Task UpdateUserAsync(User user);
    }
}
