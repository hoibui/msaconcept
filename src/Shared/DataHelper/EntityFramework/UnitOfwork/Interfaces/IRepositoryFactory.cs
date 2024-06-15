namespace DataHelper.EntityFramework.UnitOfwork.Interfaces
{
    public interface IRepositoryFactory
    {
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
    }
}
