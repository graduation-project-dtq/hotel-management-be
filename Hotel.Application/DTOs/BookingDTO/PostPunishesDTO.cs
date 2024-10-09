using System.ComponentModel.DataAnnotations;

namespace Hotel.Application.DTOs.BookingDTO
{
    public class PostPunishesDTO
    {
        [Required(ErrorMessage = "FacilitiesID không được để trống!")]
        public string FacilitiesID { get; set; } = string.Empty;
        public string? Note { get; set; }
    }
}
