using Obrazovashka.Models;
using Obrazovashka.Results;

namespace Obrazovashka.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task AddUserAsync(User user);
        Task<UserResult> GetUserByEmailAsync(string email);
        Task<UserResult> GetUserByIdAsync(int userId);
        Task UpdateUserAsync(User user);
    }
}
