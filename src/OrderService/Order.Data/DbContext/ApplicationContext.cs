using DataHelper.EntityFramework.Interfaces;
using Microsoft.EntityFrameworkCore;
using Order.Domain.Entities;

namespace Order.Data.DbContext
{
    public abstract class ApplicationContext : Microsoft.EntityFrameworkCore.DbContext, IApplicationDbContext
    {
        public ApplicationContext(DbContextOptions options) : base(options) { }


        public virtual DbSet<OrderItem> OrderItems { get; set; }
    }
}
