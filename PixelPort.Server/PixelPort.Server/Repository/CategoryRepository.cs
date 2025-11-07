using Microsoft.EntityFrameworkCore;
using PixelPort.Server.Data;
using PixelPort.Server.Models;
using PixelPort.Server.Models.DTO;
using PixelPort.Server.Repository.IRepository;
using System.Linq.Expressions;

namespace PixelPort.Server.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly PixelPortDbContext _db;
        public CategoryRepository(PixelPortDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
