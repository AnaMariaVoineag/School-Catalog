using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
namespace backend.Data
{
    public class MariaDbContext(DbContextOptions<MariaDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }

    }
}
