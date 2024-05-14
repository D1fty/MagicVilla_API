using System.Linq.Expressions;

namespace MagicVilla_VillaAPI.Repository.IRepository
{
    public interface IRepository<T>  where T : class
    {
        // Instantiated
        Task CreateAsync(T entity);

        Task RemoveAsync(T entity);

        Task SaveAsync();

        Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null);

        Task<T> GetAsync(Expression<Func<T, bool>> filter, bool tracked = true);

        Task UpdateAsync(T entity);

        Task<bool> NoneAsync(Expression<Func<T, bool>> expression);
        Task<bool> AnyAsync(Expression<Func<T, bool>> expression);
    }
}
