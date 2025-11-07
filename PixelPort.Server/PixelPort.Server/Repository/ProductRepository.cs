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
        public async Task<List<Product>> GetAllWithDetailsAsync(string search = null,
            List<int> categoryIds = null,
            List<int> manufacturerIds = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string sortBy = "name",
            bool sortDesc = false, 
            bool tracked = true)
        {
            IQueryable<Product> query = _db.Products
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(p => p.Characteristics);

            if (!tracked)
            {
                query = query.AsNoTracking();
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.ProductName.Contains(search));
            }

            if (categoryIds != null && categoryIds.Any())
            {
                query = query.Where(p => categoryIds.Contains(p.CategoryID));
            }

            if (manufacturerIds != null && manufacturerIds.Any())
            {
                query = query.Where(p => manufacturerIds.Contains(p.ManufacturerID));
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            query = sortBy.ToLower() switch
            {
                "id" => sortDesc ? query.OrderByDescending(p => p.Id) : query.OrderBy(p => p.Id),
                "price" => sortDesc ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
                "createdDate" => sortDesc ? query.OrderByDescending(p => p.CreatedDate) : query.OrderBy(p => p.CreatedDate),
                "rate" => sortDesc ? query.OrderByDescending(p => p.Rate) : query.OrderBy(p => p.Rate),
                "name" => sortDesc ? query.OrderByDescending(p => p.ProductName) : query.OrderBy(p => p.ProductName),
                _ => sortDesc ? query.OrderByDescending(p => p.ProductName) : query.OrderBy(p => p.ProductName)
            };

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
