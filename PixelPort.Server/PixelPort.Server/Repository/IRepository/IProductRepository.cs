using PixelPort.Server.Models;
using PixelPort.Server.Models.DTO;
using System.Linq.Expressions;

namespace PixelPort.Server.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<List<Product>> GetAllWithDetailsAsync(Expression<Func<Product, bool>>? filter = null, bool tracked = true);
        Task<Product> GetWithDetailsAsync(Expression<Func<Product, bool>>? filter = null, bool tracked = true);
        Task UpdateWithCharacteristicsAsync(Product product, List<ProductCharacteristic>? characteristics);
    }
}
