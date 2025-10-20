using PixelPort.Server.Models.Dto;
using System.ComponentModel.DataAnnotations;

namespace PixelPort.Server.Models.DTO
{
    public class ProductUpdateDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string ProductName { get; set; }
        [Required]
        public int CategoryID { get; set; }
        [Required]
        public int ManufacturerID { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int Price { get; set; }

        public List<ProductCharacteristicCreateDTO> Characteristics { get; set; } = new();
    }
}
