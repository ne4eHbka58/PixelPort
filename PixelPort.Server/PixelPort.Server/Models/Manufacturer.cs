using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PixelPort.Server.Models
{
    public class Manufacturer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string ManufacturerName { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}
