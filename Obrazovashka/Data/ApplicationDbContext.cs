using Microsoft.EntityFrameworkCore;
using Obrazovashka.Models;

namespace Obrazovashka.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<User>? Users { get; set; }
        public DbSet<Course>? Courses { get; set; }
        public DbSet<Certificate>? Certificates { get; set; }
        public DbSet<Completion>? Completions { get; set; }
        public DbSet<Enrollment>? Enrollments { get; set; }
        public DbSet<Feedback>? Feedbacks { get; set; }
        public DbSet<Review>? Reviews { get; set; }
    }
}
