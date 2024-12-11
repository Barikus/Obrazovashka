using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Obrazovashka.Models;

namespace Obrazovashka.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task AddUserAsync(User user);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> UpdateUserAsync(User user);
    }
}
