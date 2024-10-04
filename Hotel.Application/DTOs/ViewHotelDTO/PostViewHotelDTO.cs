

using System.ComponentModel.DataAnnotations;

namespace Hotel.Application.DTOs.ViewHotelDTO
{
    public class PostViewHotelDTO
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public decimal Price { get; set; }
    }
}
