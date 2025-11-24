using Microsoft.EntityFrameworkCore;
using PixelPort.Server.Data;
using PixelPort.Server.Helpers;
using PixelPort.Server.Models;
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
       
        public async Task<PagedResult<Product>> GetAllWithDetailsAsync(string search = null,
            int? categoryId = null,
            List<int> manufacturerIds = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string sortBy = "name",
            bool sortDesc = false,
            int page = 0,
            int pageSize = 24,
            bool tracked = true,
            CancellationToken ct = default)
        {
            // Запрос для данных
            IQueryable<Product> dataQuery = _db.Products
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(p => p.Characteristics);

            if (!tracked)
            {
                dataQuery = dataQuery.AsNoTracking();
            }

            // Поиск
            if (!string.IsNullOrEmpty(search))
            {
                dataQuery = dataQuery.Where(p => p.ProductName.Contains(search));
            }

            #region Фильтры
            if (categoryId != null)
            {
                dataQuery = dataQuery.Where(p => p.CategoryID == categoryId);
            }

            if (manufacturerIds != null && manufacturerIds.Any())
            {
                dataQuery = dataQuery.Where(p => manufacturerIds.Contains(p.ManufacturerID));
            }

            if (minPrice.HasValue)
            {
                dataQuery = dataQuery.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                dataQuery = dataQuery.Where(p => p.Price <= maxPrice.Value);
            }
            #endregion

            // Получаем общее количество ДО пагинации
            int totalCount = await dataQuery.CountAsync(ct);
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            // Проверка на валидность страницы
            if (page > totalPages && totalPages > 0)
            {
                return new PagedResult<Product>
                {
                    Items = new List<Product>(),
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalCount = totalCount
                };
            }

            // Сортировка
            dataQuery = sortBy.ToLower() switch
            {
                "id" => sortDesc ? dataQuery.OrderByDescending(p => p.Id) : dataQuery.OrderBy(p => p.Id),
                "price" => sortDesc ? dataQuery.OrderByDescending(p => p.Price) : dataQuery.OrderBy(p => p.Price),
                "createdDate" => sortDesc ? dataQuery.OrderByDescending(p => p.CreatedDate) : dataQuery.OrderBy(p => p.CreatedDate),
                "rate" => sortDesc ? dataQuery.OrderByDescending(p => p.Rate) : dataQuery.OrderBy(p => p.Rate),
                "name" => sortDesc ? dataQuery.OrderByDescending(p => p.ProductName) : dataQuery.OrderBy(p => p.ProductName),
                _ => sortDesc ? dataQuery.OrderByDescending(p => p.ProductName) : dataQuery.OrderBy(p => p.ProductName)
            };


            // Пагинация
            var items = await dataQuery
                .Skip(page * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new PagedResult<Product>
            {
                Items = items,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<Product> GetWithDetailsAsync(Expression<Func<Product, bool>>? filter = null, bool tracked = true, CancellationToken ct = default)
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

            return await query.FirstOrDefaultAsync(ct);
        }

        public async Task<Product> UpdateWithCharacteristicsAsync(Product product, List<ProductCharacteristic>? characteristics, CancellationToken ct = default)
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
                await _db.ProductCharacteristics.AddRangeAsync(characteristics, ct);

            }

            // Обновляем продукт
            product.UpdatedDate = DateTime.Now;
            _db.Products.Update(product);
            await _db.SaveChangesAsync(ct);

            // Перезагружаем с базы
            await _db.Entry(product).ReloadAsync(ct);
            return product;
        }

    }
}
