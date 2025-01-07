using Microsoft.EntityFrameworkCore;
using Obrazovashka.Data;
using Obrazovashka.Models;
using Obrazovashka.Repositories.Interfaces;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly ApplicationDbContext _context;

    public EnrollmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddEnrollmentAsync(Enrollment enrollment)
    {
        try
        {
            await _context.Enrollments!.AddAsync(enrollment);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при добавлении записи: {ex.Message}");
            throw;
        }
    }

    public async Task<IList<Enrollment>> GetEnrollmentsByUserIdAsync(int userId)
    {
        try
        {
            return await _context.Enrollments!.Where(e => e.UserId == userId).ToListAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при получении записей пользователя: {ex.Message}");
            throw;
        }
    }

    public async Task<Enrollment> GetEnrollmentAsync(int userId, int courseId)
    {
        try
        {
            var enrollment = await _context.Enrollments!
                .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);

            return enrollment!;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при поиске записи: {ex.Message}");
            throw;
        }
    }


    public async Task UpdateEnrollmentAsync(Enrollment enrollment)
    {
        try
        {
            _context.Enrollments!.Update(enrollment);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при обновлении записи: {ex.Message}");
            throw;
        }
    }
}