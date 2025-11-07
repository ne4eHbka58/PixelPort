using PixelPort.Server.Models;
using PixelPort.Server.Models.DTO;
using System.Linq.Expressions;

namespace PixelPort.Server.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<List<Product>> GetAllWithDetailsAsync(string search = null,
            List<int> categoryIds = null,
            List<int> manufacturerIds = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string sortBy = "name",
            bool sortDesc = false,
            bool tracked = true);
        Task<Product> GetWithDetailsAsync(Expression<Func<Product, bool>>? filter = null, bool tracked = true);
        Task<Product> UpdateWithCharacteristicsAsync(Product product, List<ProductCharacteristic>? characteristics);
    }
}
