﻿using PixelPort.Server.Data;
using PixelPort.Server.Models;
using PixelPort.Server.Repository.IRepository;

namespace PixelPort.Server.Repository
{
    public class ProductCharacteristicRepository : Repository<ProductCharacteristic>, IProductCharacteristicRepository
    {
        private readonly PixelPortDbContext _db;
        public ProductCharacteristicRepository(PixelPortDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<ProductCharacteristic> UpdateCharacteristicAsync(ProductCharacteristic characteristic)
        {
            _db.ProductCharacteristics.Update(characteristic);
            await SaveAsync();

            // Перезагружаем с базы
            await _db.Entry(characteristic).ReloadAsync();
            return characteristic;
        }
    }
}
