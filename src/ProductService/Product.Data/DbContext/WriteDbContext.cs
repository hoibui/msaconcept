using Microsoft.EntityFrameworkCore;

namespace Product.Data.DbContext
{
    public class WriteDbContext : ApplicationContext
    {
        public WriteDbContext(DbContextOptions<WriteDbContext> options) : base(options)
        {
        }
    }
}