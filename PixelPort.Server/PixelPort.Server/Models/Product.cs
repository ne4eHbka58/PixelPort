using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PixelPort.Server.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string ProductName { get; set; }
        [ForeignKey("Category")]
        public int CategoryID { get; set; }
        public Category Category { get; set; }
        [ForeignKey("Manufacturer")]
        public int ManufacturerID { get; set; }
        public Manufacturer Manufacturer { get; set; }
        public int Price { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public ICollection<ProductCharacteristic> Characteristics { get; set; }

    }
}
