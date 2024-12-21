using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Obrazovashka.Data;
using Obrazovashka.Models;
using Obrazovashka.Repositories.Interfaces;
using Obrazovashka.Results;

namespace Obrazovashka.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {   
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<UserResult> GetUserByIdAsync(int userId)
        {
            var user = _context.Users.First(u => u.Id == userId);
            if (user == null)
                return new UserResult() { Success = false, Message = "Пользователь не найден." };

            var userResult = new UserResult
            {
                Success = true,
                Message = "Пользователь найден",
                User = user
            };

            return userResult;
        }

        public async Task<UserResult> GetUserByEmailAsync(string email)
        {
            var user = _context.Users.First(u => u.Email == email);
            if (user == null)
                return new UserResult() { Success = false, Message = "Пользователь не найден." };

            var userResult = new UserResult
            {
                Success = true,
                Message = "Пользователь найден",
                User = user
            };

            return userResult;
        }
    }
}
