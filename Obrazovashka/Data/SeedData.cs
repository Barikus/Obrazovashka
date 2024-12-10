using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Obrazovashka.Models;
using System.Linq;

namespace Obrazovashka.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                if (!context.Users.Any())
                {
                    context.Users.Add(new User { Username = "Admin", Email = "admin@example.com", PasswordHash = "hashedpassword" });
                    context.SaveChanges();
                }
            }
        }
    }
}
