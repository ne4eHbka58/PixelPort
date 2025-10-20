using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PixelPort.Server.Models
{
    public class ProductCharacteristic
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public string CharacteristicName { get; set; }
        public string CharacteristicValue { get; set; }
    }
}
