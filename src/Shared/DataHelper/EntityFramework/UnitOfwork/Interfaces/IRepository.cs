using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using Paginate;

namespace DataHelper.EntityFramework.UnitOfwork.Interfaces
{
    public interface IRepository<T> : IDisposable where T : class
    {
        IQueryable<T> GetQueryable(bool asNoTracking = false);

        Task<ICollection<TResult>> GetListAsync<TResult>(
            Expression<Func<T, TResult>> selector,
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            int? take = null,
            bool asNoTracking = true,
            bool ignoreQueryFilters = false,
            CancellationToken cancellationToken = default) where TResult : class;

        Task<T> SingleOrDefaultAsync(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
            bool asNoTracking = true,
            bool ignoreQueryFilters = false,
            CancellationToken cancellationToken = default);

        Task<ICollection<T>> GetListAsync(
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            int? take = null,
            bool asNoTracking = true,
            CancellationToken cancellationToken = default);
        
        Task<IPaginate<TResult>> GetPagingListAsync<TResult>(
            Expression<Func<T, TResult>> selector,
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            int page = 1,
            int size = 20,
            bool asNoTracking = true,
            CancellationToken cancellationToken = default);
        
        T Update(T entity);
        void Update(params T[] entities);
        void Update(IEnumerable<T> entities);

        ValueTask<EntityEntry<T>> InsertAsync(T entity, CancellationToken cancellationToken = default);
        Task InsertAsync(params T[] entities);
        Task InsertAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
        void DeleteAll(IEnumerable<T> entities);
    }
}