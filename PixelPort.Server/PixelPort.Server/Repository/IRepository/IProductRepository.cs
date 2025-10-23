using PixelPort.Server.Models;
using PixelPort.Server.Models.DTO;
using System.Linq.Expressions;

namespace PixelPort.Server.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<Product> GetWithCharacteristicsAsync(Expression<Func<Product, bool>> filter, bool tracked = true);
        Task UpdateWithCharacteristicsAsync(Product product, List<ProductCharacteristic>? characteristics);
    }
}
