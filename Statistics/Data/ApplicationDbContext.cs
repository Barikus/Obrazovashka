using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Statistics.Models;

namespace Statistics.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<GlobalStatistics>? GlobalStatistics { get; set; }
        public DbSet<CourseStatistics>? CourseStatistics { get; set; }
        public DbSet<UserStatistics>? UserStatistics { get; set; }
    }
}
