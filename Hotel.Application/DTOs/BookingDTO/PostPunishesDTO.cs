using System.ComponentModel.DataAnnotations;

namespace Hotel.Application.DTOs.BookingDTO
{
    public class PostPunishesDTO
    {
        [Required(ErrorMessage = "Không được để trống mã đồ dùng, tài sản!")]
        public string FacilitiesID { get; set; } = string.Empty;
        [Required(ErrorMessage = "Không được để trống tiền phạt, tài sản!")]
        public decimal Fine {  get; set; }
        public int Quantity {  get; set; }
    }
}
