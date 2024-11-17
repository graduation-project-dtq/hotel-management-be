

using System.ComponentModel.DataAnnotations;

namespace Hotel.Application.DTOs.OverviewDTO
{
    public class PostOverviewDTO
    {
        [Required(ErrorMessage = "Bạn chưa đánh giá nhân viên của khách sạn")]
        public float EmployeePoint { get; set; }  //Đánh giá nhân viên
        [Required(ErrorMessage = "Bạn chưa đánh giá tiện nghi của khách sạn")]
        public float ComfortPoint { get; set; } //Đánh giá tiện nghi
        [Required(ErrorMessage = "Bạn chưa đánh giá độ sạch sẽ của khách sạn")]
        public float ClearPoint { get; set; } //Sạch sẽ
        [Required(ErrorMessage = "Bạn chưa đánh giá dịch vụ của khách sạn")]
        public float ServicePoint { get; set; } //Dịch vụ
        [Required(ErrorMessage = "Bạn chưa đánh giá View của khách sạn")]
        public float ViewPoint { get; set; } //View
        [Required(ErrorMessage = "Bạn chưa đánh giá phòng của khách sạn")]
        public float RoomPoint { get; set; } //Phòng
        [Required(ErrorMessage = "Vui lòng đăng nhập")]
        public string CustomerId { get; set; } = string.Empty;
    }
}
