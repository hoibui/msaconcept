using Microsoft.EntityFrameworkCore;

namespace Order.Data.DbContext
{
    public class ReadDbContext : ApplicationContext
    {
        public ReadDbContext(DbContextOptions<ReadDbContext> options) : base(options)
        {
        }
    }
}