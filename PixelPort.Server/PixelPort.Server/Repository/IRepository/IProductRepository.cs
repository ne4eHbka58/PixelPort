using PixelPort.Server.Helpers;
using PixelPort.Server.Models;
using System.Linq.Expressions;

namespace PixelPort.Server.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<PagedResult<Product>> GetAllWithDetailsAsync(string search = null,
            int? categoryId = null,
            List<int> manufacturerIds = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string sortBy = "name",
            bool sortDesc = false,
            int page = 0,
            int pageSize = 24,
            bool tracked = true,
            CancellationToken ct = default);
        Task<Product> GetWithDetailsAsync(Expression<Func<Product, bool>>? filter = null, bool tracked = true, CancellationToken ct = default);
        Task<Product> UpdateWithCharacteristicsAsync(Product product, List<ProductCharacteristic>? characteristics, CancellationToken ct = default);
    }
}
