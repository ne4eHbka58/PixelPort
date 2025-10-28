using PixelPort.Server.Models.Dto;
using System.ComponentModel.DataAnnotations;
using System.Reflection.PortableExecutable;

namespace PixelPort.Server.Models.DTO
{
    public class ProductCreateDTO
    {
        [Required]
        public string ProductName { get; set; }
        [Required]
        public int CategoryID { get; set; }
        [Required]
        public int ManufacturerID { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int Price { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public double Rate { get; set; }

        public List<ProductCharacteristicCreateDTO> Characteristics { get; set; } = new();
    }
}
