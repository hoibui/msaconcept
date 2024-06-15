using System.Linq.Expressions;
using DataHelper.EntityFramework.UnitOfwork.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using Paginate;

namespace DataHelper.EntityFramework.UnitOfwork.Implements
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly Microsoft.EntityFrameworkCore.DbContext _dbContext;
        protected readonly DbSet<T> _dbSet;

        public Repository(Microsoft.EntityFrameworkCore.DbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<T>();
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }

        public IQueryable<T> GetQueryable(bool asNoTracking = false)
        {
            var queryable = _dbContext.Set<T>().AsQueryable();

            if (asNoTracking)
            {
                queryable = queryable.AsNoTracking();
            }

            return queryable;
        }


        public async Task<ICollection<T>> GetListAsync(
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            int? take = null,
            bool asNoTracking = true,
            CancellationToken cancellationToken = default)
        {

            IQueryable<T> query = _dbSet;
            if (asNoTracking) query = query.AsNoTracking();

            if (include != null) query = include(query);

            if (predicate != null) query = query.Where(predicate);

            if (orderBy != null)
                return await orderBy(query).ToListAsync(cancellationToken);

            if (take.HasValue && take.Value > 0)
            {
                query = query.Take(take.Value);
            }

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<ICollection<TResult>> GetListAsync<TResult>(
            Expression<Func<T, TResult>> selector,
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            int? take = null,
            bool asNoTracking = true,
            bool ignoreQueryFilters = false,
            CancellationToken cancellationToken = default) where TResult : class
        {
            IQueryable<T> query = _dbSet;

            if (asNoTracking) query = query.AsNoTracking();

            if (include != null) query = include(query);

            if (predicate != null) query = query.Where(predicate);

            if (ignoreQueryFilters) query = query.IgnoreQueryFilters();

            if (orderBy != null)
                return await orderBy(query).Select(selector).ToListAsync(cancellationToken);

            if (take.HasValue && take.Value > 0)
            {
                query = query.Take(take.Value);
            }

            return await query.Select(selector).ToListAsync(cancellationToken);
        }

        public async Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, bool asNoTracking = true, bool ignoreQueryFilters = false, CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _dbSet;

            if (asNoTracking) query = query.AsNoTracking();

            if (include != null) query = include(query);

            if (predicate != null) query = query.Where(predicate);

            if (ignoreQueryFilters) query = query.IgnoreQueryFilters();

            if (orderBy != null) return await orderBy(query).FirstOrDefaultAsync();

            return await query.FirstOrDefaultAsync(cancellationToken);
        }


        public Task<IPaginate<TResult>> GetPagingListAsync<TResult>(
            Expression<Func<T, TResult>> selector,
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            int page = 1,
            int size = 20,
            bool asNoTracking = true,
            CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _dbSet;

            if (asNoTracking) query = query.AsNoTracking();

            if (include != null) query = include(query);

            if (predicate != null) query = query.Where(predicate);

            if (orderBy != null)
                return orderBy(query).Select(selector).ToPaginateAsync(page, size, 1, cancellationToken);

            return query.Select(selector).ToPaginateAsync(page, size, 1, cancellationToken);
        }
        
        public T Update(T entity)
        {
            var entry = _dbContext.Entry(entity);
            entry.State = EntityState.Modified;
            return _dbSet.Update(entity).Entity;
        }

        public void Update(params T[] entities)
        {
            _dbSet.UpdateRange(entities);
        }

        public void Update(IEnumerable<T> entities)
        {
            _dbSet.UpdateRange(entities);
        }


        public virtual ValueTask<EntityEntry<T>> InsertAsync(T entity, CancellationToken cancellationToken = default)
        {
            return _dbSet.AddAsync(entity, cancellationToken);
        }

        public virtual Task InsertAsync(params T[] entities)
        {
            return _dbSet.AddRangeAsync(entities);
        }

        public virtual Task InsertAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            return _dbSet.AddRangeAsync(entities, cancellationToken);
        }

        public void DeleteAll(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }
    }
}