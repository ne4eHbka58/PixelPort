using Microsoft.EntityFrameworkCore;
using PixelPort.Server.Data;
using PixelPort.Server.Repository.IRepository;
using System.Linq.Expressions;
using System.Threading;

namespace PixelPort.Server.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly PixelPortDbContext _db;
        internal DbSet<T> dbSet;
        public Repository(PixelPortDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
        }
        public async Task<T> CreateAsync(T entity, CancellationToken ct = default)
        {
            await dbSet.AddAsync(entity);
            await SaveAsync(ct);

            // Перезагружаем с базы
            await _db.Entry(entity).ReloadAsync(ct);
            return entity;
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>>? filter = null, bool tracked = true, CancellationToken ct = default)
        {
            IQueryable<T> query = dbSet;

            if (!tracked)
            {
                query = query.AsNoTracking();
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.FirstOrDefaultAsync(ct);
        }


        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, CancellationToken ct = default)
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.ToListAsync(ct);
        }

        public async Task RemoveAsync(T entity, CancellationToken ct = default)
        {
            dbSet.Remove(entity);
            await SaveAsync(ct);
        }

        public async Task SaveAsync(CancellationToken ct = default)
        {
            await _db.SaveChangesAsync(ct);
        }
    }
}
