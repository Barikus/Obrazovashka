using Microsoft.EntityFrameworkCore;
using Obrazovashka.Models;
using Obrazovashka.Repositories.Interfaces;
using Obrazovashka.Results;
using Obrazovashka.Data;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddUserAsync(User user)
    {
        try
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при добавлении пользователя: {ex.Message}");
            throw;
        }
    }

    public async Task UpdateUserAsync(User user)
    {
        try
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при обновлении пользователя: {ex.Message}");
            throw;
        }
    }

    public async Task<UserResult> GetUserByIdAsync(int userId)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return new UserResult { Success = false, Message = "Пользователь не найден." };
            }

            return new UserResult { Success = true, Message = "Пользователь найден.", User = user };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при получении пользователя: {ex.Message}");
            throw;
        }
    }

    public async Task<UserResult> GetUserByEmailAsync(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        // Проверяем, найден ли пользователь
        if (user == null)
        {
            return new UserResult() { Success = false, Message = "Пользователь не найден." };
        }

        var userResult = new UserResult
        {
            Success = true,
            Message = "Пользователь найден",
            User = user
        };

        return userResult;
    }
}
