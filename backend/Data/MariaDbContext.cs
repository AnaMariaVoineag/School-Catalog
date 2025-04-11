using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
namespace backend.Data
{
    public class MariaDbContext(DbContextOptions<MariaDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Grade> Grades { get; set; }

    }
}
