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
       
        public async Task UpdateWithCharacteristicsAsync(Product product, List<ProductCharacteristic>? characteristics)
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
        }

        public async Task<Product> GetWithCharacteristicsAsync(Expression<Func<Product, bool>> filter, bool tracked = true)
        {
            IQueryable<Product> query = _db.Products.Include(p => p.Characteristics);

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
