namespace DataHelper.EntityFramework.UnitOfwork.Interfaces
{
    public interface IUnitOfWork : IRepositoryFactory, IDisposable
    {
        void CommitWithUpdateConcurrencyConflicts();
        int Commit();
        Task<int> CommitAsync();
    }

    public interface IUnitOfWork<TContext> : IUnitOfWork where TContext : Microsoft.EntityFrameworkCore.DbContext
    {
        TContext Context { get; }
    }
}
