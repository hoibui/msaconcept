using DataHelper.EntityFramework.Interfaces;
using Microsoft.EntityFrameworkCore;
using Product.Domain.Entities;

namespace Product.Data.DbContext
{
    public abstract class ApplicationContext : Microsoft.EntityFrameworkCore.DbContext, IApplicationDbContext
    {
        public ApplicationContext(DbContextOptions options) : base(options) { }


        public virtual DbSet<Domain.Entities.Product> Products { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
    }
}
