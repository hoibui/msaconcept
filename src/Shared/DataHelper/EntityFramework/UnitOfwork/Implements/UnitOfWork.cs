using DataHelper.EntityFramework.UnitOfwork.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataHelper.EntityFramework.UnitOfwork.Implements
{
    public class UnitOfWork<TContext> : IUnitOfWork<TContext> where TContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public TContext Context { get; }

        private Dictionary<Type, object> _repositories;

        public UnitOfWork(TContext context)
        {
            Context = context;
        }

        public int Commit()
        {
            return Context.SaveChanges();
        }

        public async Task<int> CommitAsync()
        {
            return await Context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Context?.Dispose();
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            _repositories ??= new Dictionary<Type, object>();

            if (_repositories.TryGetValue(typeof(TEntity), out object repository))
            {
                return (IRepository<TEntity>)repository;
            }

            repository = new Repository<TEntity>(Context);

            _repositories.Add(typeof(TEntity), repository);

            return (IRepository<TEntity>)repository;
        }

        public void CommitWithUpdateConcurrencyConflicts()
        {
            bool saveFailed;
            do
            {
                saveFailed = false;
                try
                {
                    Context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;

                    // Update original values from the database
                    var entry = ex.Entries.Single();
                    entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                }

            } while (saveFailed);
        }
    }
}
