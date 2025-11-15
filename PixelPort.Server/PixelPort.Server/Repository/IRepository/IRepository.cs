using System.Linq.Expressions;

namespace PixelPort.Server.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, CancellationToken ct = default);
        Task<T> GetAsync(Expression<Func<T, bool>>? filter = null, bool tracked = true, CancellationToken ct = default);
        Task<T> CreateAsync(T entity, CancellationToken ct = default);
        Task RemoveAsync(T entity, CancellationToken ct = default);
        Task SaveAsync(CancellationToken ct = default);
    }
}
