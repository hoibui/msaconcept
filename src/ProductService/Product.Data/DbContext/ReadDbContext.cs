using Microsoft.EntityFrameworkCore;

namespace Product.Data.DbContext
{
    public class ReadDbContext : ApplicationContext
    {
        public ReadDbContext(DbContextOptions<ReadDbContext> options) : base(options)
        {
        }
    }
}