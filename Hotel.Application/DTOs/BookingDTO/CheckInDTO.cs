using System.ComponentModel.DataAnnotations;

namespace Hotel.Application.DTOs.BookingDTO
{
    public class CheckInDTO
    {
        [Required(ErrorMessage ="Không được để trống mã Booking")]
        public string BookingId { get; set; } = string.Empty;
        [Required(ErrorMessage = "Không được để trống tên khách hàng")]
        public string CustomerName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Không được để trống số CMND/CCCD")]
        public string IdentityCard {  get; set; } = string.Empty;
    }
}
