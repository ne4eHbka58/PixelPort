using Microsoft.EntityFrameworkCore;
using PixelPort.Server.Models;

namespace PixelPort.Server.Data
{
    public class PixelPortDbContext : DbContext
    {
        public PixelPortDbContext(DbContextOptions<PixelPortDbContext> options) 
            : base(options)
        {
            
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCharacteristic> ProductCharacteristics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .HasMany(p => p.Characteristics)
                .WithOne(pc => pc.Product)
                .HasForeignKey(pc => pc.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Category>().HasData(
                new Category()
                {
                    Id = 1,
                    CategoryName = "Телефоны"
                }
            );
            modelBuilder.Entity<Manufacturer>().HasData(
                new Manufacturer()
                {
                    Id = 1,
                    ManufacturerName = "Apple"
                },
                new Manufacturer()
                {
                    Id = 2,
                    ManufacturerName = "Samsung"
                }
            );
            modelBuilder.Entity<Product>().HasData(
                new Product()
                {
                    Id = 1,
                    ProductName = "Iphone 14 128 Гб Чёрный ",
                    CategoryID = 1,
                    ManufacturerID = 1,
                    Price = 40000,
                    ImageUrl = "",
                    Rate = 0,
                    CreatedDate = new DateTime(2025, 10, 20),
                    UpdatedDate = new DateTime(2025, 10, 20),
                },
                new Product()
                {
                    Id = 2,
                    ProductName = "Sumsung Galaxy A26 128 Гб Зелёный",
                    CategoryID = 1,
                    ManufacturerID = 2,
                    Price = 20000,
                    ImageUrl = "",
                    Rate = 0,
                    CreatedDate = new DateTime(2025, 10, 20),
                    UpdatedDate = new DateTime(2025, 10, 20),
                }
            );
            modelBuilder.Entity<ProductCharacteristic>().HasData(
                new ProductCharacteristic()
                {
                    Id = 1,
                    ProductId = 1,
                    CharacteristicName = "Объём встроенной памяти",
                    CharacteristicValue = "128 Гб"
                },
                new ProductCharacteristic()
                {
                    Id = 2,
                    ProductId = 1,
                    CharacteristicName = "Объём оперативной памяти",
                    CharacteristicValue = "6 Гб"
                },
                new ProductCharacteristic()
                {
                    Id = 3,
                    ProductId = 1,
                    CharacteristicName = "Цвет",
                    CharacteristicValue = "Чёрный"
                },
                new ProductCharacteristic()
                {
                    Id = 4,
                    ProductId = 1,
                    CharacteristicName = "Количество основных камер",
                    CharacteristicValue = "2"
                },
                new ProductCharacteristic()
                {
                    Id = 5,
                    ProductId = 1,
                    CharacteristicName = "Диагональ экрана",
                    CharacteristicValue = "6.1"
                },
                new ProductCharacteristic()
                {
                    Id = 6,
                    ProductId = 2,
                    CharacteristicName = "Объём встроенной памяти",
                    CharacteristicValue = "128 Гб"
                },
                new ProductCharacteristic()
                {
                    Id = 7,
                    ProductId = 2,
                    CharacteristicName = "Объём оперативной памяти",
                    CharacteristicValue = "6 Гб"
                },
                new ProductCharacteristic()
                {
                    Id = 8,
                    ProductId = 2,
                    CharacteristicName = "Цвет",
                    CharacteristicValue = "Зелёный"
                },
                new ProductCharacteristic()
                {
                    Id = 9,
                    ProductId = 2,
                    CharacteristicName = "Количество основных камер",
                    CharacteristicValue = "3"
                },
                new ProductCharacteristic()
                {
                    Id = 10,
                    ProductId = 2,
                    CharacteristicName = "Диагональ экрана",
                    CharacteristicValue = "6.7"
                }
            );
        }
    }
}
