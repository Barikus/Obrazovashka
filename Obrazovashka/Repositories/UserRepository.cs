using Obrazovashka.Data;
using Obrazovashka.AuthService.Models;
using Obrazovashka.AuthService.Repositories.Interfaces;
using Obrazovashka.AuthService.Results;
using Microsoft.EntityFrameworkCore;

namespace Obrazovashka.AuthService.Repositories
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
            var user = _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return new UserResult() { Success = false, Message = "Пользователь не найден." };

            var userResult = new UserResult
            {
                Success = true,
                Message = "Пользователь найден",
                User = await user
            };

            return userResult;
        }

        public async Task<UserResult> GetUserByEmailAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            // Проверяем, найден ли пользователь
            if (user == null)
            {
                return new UserResult() { Success = false, Message = "Пользователь не найден." };
            }

            // Возвращаем успешный результат
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
