using Microsoft.EntityFrameworkCore;

namespace Order.Data.DbContext
{
    public class WriteDbContext : ApplicationContext
    {
        public WriteDbContext(DbContextOptions<WriteDbContext> options) : base(options)
        {
        }
    }
}