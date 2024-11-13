using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Hotel.Application.DTOs.RoomTypeDetailDTO
{
    public class PostRoomTypeDetailDTO
    {
        [FromForm]
        [Required(ErrorMessage = "Vui lòng nhập mã loại phòng!")]
        public string RoomTypeID { get; set; } = string.Empty;

        [FromForm]
        [Required(ErrorMessage = "Vui lòng nhập tên loại phòng!")]
        [StringLength(100, ErrorMessage = "Tên loại phòng không được vượt quá 100 ký tự.")]
        public string Name { get; set; } = string.Empty;

        [FromForm]
        [Required(ErrorMessage = "Vui lòng nhập sức chứa tối đa!")]
        [Range(1, int.MaxValue, ErrorMessage = "Sức chứa tối đa phải lớn hơn 0.")]
        public int CapacityMax { get; set; }

        [FromForm]
        [Required(ErrorMessage = "Vui lòng nhập diện tích phòng!")]
        [Range(1, double.MaxValue, ErrorMessage = "Diện tích phòng phải lớn hơn 0.")]
        public decimal Area { get; set; }

        [FromForm]
        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
        public string? Description { get; set; }

        [FromForm]
        [Required(ErrorMessage = "Vui lòng nhập giá cơ bản!")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá cơ bản phải lớn hơn hoặc bằng 0.")]
        public decimal BasePrice { get; set; }
    }
}
