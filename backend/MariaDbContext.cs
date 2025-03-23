using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
namespace backend
{
    public class MariaDbContext : DbContext
    {
        public MariaDbContext(DbContextOptions<MariaDbContext> options)
            : base(options) { }

    }
}
