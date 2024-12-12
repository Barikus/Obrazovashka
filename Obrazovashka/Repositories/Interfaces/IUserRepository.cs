using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Obrazovashka.Models;

namespace Obrazovashka.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task AddUserAsync(User user);
        Task<User> GetUserByIdAsync(int id);
        Task<User> GetUserByEmailAsync(string email);
        Task UpdateUserAsync(User user);
    }
}
