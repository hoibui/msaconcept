using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DataHelper.EntityFramework.Interfaces
{
    public interface IApplicationDbContext
    {
        ChangeTracker ChangeTracker { get; }
        IModel Model { get; }
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
    }
}
