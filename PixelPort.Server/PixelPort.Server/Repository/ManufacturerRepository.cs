using PixelPort.Server.Data;
using PixelPort.Server.Models;
using PixelPort.Server.Repository.IRepository;

namespace PixelPort.Server.Repository
{
    public class ManufacturerRepository : Repository<Manufacturer>, IManufacturerRepository
    {
        private readonly PixelPortDbContext _db;
        public ManufacturerRepository(PixelPortDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
