using Microsoft.EntityFrameworkCore;
using PixelPort.Server.Data;
using PixelPort.Server.Models;
using PixelPort.Server.Models.DTO;
using PixelPort.Server.Repository.IRepository;
using System.Linq.Expressions;

namespace PixelPort.Server.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly PixelPortDbContext _db;
        public ProductRepository(PixelPortDbContext db) : base(db)
        {
            _db = db;
        }
       
        public async Task<Product> UpdateWithCharacteristicsAsync(Product product, List<ProductCharacteristic>? characteristics)
        {
            // Удаляем старые характеристики
            var existingCharacteristics = _db.ProductCharacteristics.Where(pc => pc.ProductId == product.Id);
            _db.ProductCharacteristics.RemoveRange(existingCharacteristics);

            // Добавляем новые
            if (characteristics != null && characteristics.Any())
            {
                foreach (var characteristic in characteristics)
                {
                    characteristic.ProductId = product.Id;
                }
                await _db.ProductCharacteristics.AddRangeAsync(characteristics);

            }

            // Обновляем продукт
           product.UpdatedDate = DateTime.Now;
            _db.Products.Update(product);
            await _db.SaveChangesAsync();

            // Перезагружаем с базы
            await _db.Entry(product).ReloadAsync();
            return product;
        }
        public async Task<List<Product>> GetAllWithDetailsAsync(Expression<Func<Product, bool>>? filter = null, bool tracked = true)
        {
            IQueryable<Product> query = _db.Products
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(p => p.Characteristics);

            if (!tracked)
            {
                query = query.AsNoTracking();
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.ToListAsync();
        }

        public async Task<Product> GetWithDetailsAsync(Expression<Func<Product, bool>>? filter = null, bool tracked = true)
        {
            IQueryable<Product> query = _db.Products
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(p => p.Characteristics); 

            if (!tracked)
            {
                query = query.AsNoTracking();
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.FirstOrDefaultAsync();
        }
    }
}
