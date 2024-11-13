using System.ComponentModel.DataAnnotations;

namespace Hotel.Application.DTOs.BookingDTO
{
    public class PostPunishesDTO
    {
        [Required(ErrorMessage = "Không được để trống mã đồ dùng, tài sản!")]
        public string FacilitiesID { get; set; } = string.Empty;
        [Required(ErrorMessage = "Số lượng không hợp lệ!")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 1.")]
        public int Quantity {  get; set; }
    }
}
