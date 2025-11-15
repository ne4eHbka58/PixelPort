using PixelPort.Server.Models;

namespace PixelPort.Server.Repository.IRepository
{
    public interface IProductCharacteristicRepository : IRepository<ProductCharacteristic>
    {
        Task<ProductCharacteristic> UpdateCharacteristicAsync(ProductCharacteristic characteristic, CancellationToken ct = default);
    }
}
