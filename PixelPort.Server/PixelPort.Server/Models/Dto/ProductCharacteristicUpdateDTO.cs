using System.ComponentModel.DataAnnotations;

namespace PixelPort.Server.Models.Dto
{
    public class ProductCharacteristicUpdateDTO
    {
        public int Id { get; set; }
        [Required]
        public string CharacteristicName { get; set; }
        [Required]
        public string CharacteristicValue { get; set; }
    }
}
