using System.ComponentModel.DataAnnotations;

namespace Hotel.Application.DTOs.PriceAdjustmentPlanDTO
{
    public class PostPriceAdjustmentPlanDTO
    {
        [Required(ErrorMessage ="Vui lòng nhập tên kế hoạch điều chỉnh giá phòng!")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Vui lòng nhập giá tiền điều chỉnh giá phòng!")]
        public decimal AdjustmentValue { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập ngày bắt đầu điều chỉnh giá phòng!")]
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập ngày kết thúc điều chỉnh giá phòng!")]
        public DateTime EndDate { get; set; }
        public string? Description { get; set; } = string.Empty;
        public virtual ICollection<PostRoomPriceAdjustmentPlanDTO> ? RoomPriceAdjustmentPlans { get; set; }
    }
}
