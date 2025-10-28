using PixelPort.Server.Models.Dto;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PixelPort.Server.Models.DTO
{
    public class ProductResponseDTO
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public int ManufacturerID { get; set; }
        public string ManufacturerName { get; set; }
        public int Price { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public double Rate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public List<ProductCharacteristicResponseDTO> Characteristics { get; set; } = new();
    }
}
