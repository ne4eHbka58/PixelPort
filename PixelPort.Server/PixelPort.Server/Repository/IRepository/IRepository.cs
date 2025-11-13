using System.Linq.Expressions;

namespace PixelPort.Server.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null);
        Task<T> GetAsync(Expression<Func<T, bool>>? filter = null, bool tracked = true);
        Task<T> CreateAsync(T entity);
        Task RemoveAsync(T entity);
        Task SaveAsync();
    }
}
