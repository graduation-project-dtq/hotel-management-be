using Hotel.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Hotel.Application.DTOs.NotificationDTO
{
    public class PostNotificationDTO
    {
        [Required(ErrorMessage ="Vui lòng nhập mã khách hàng")]
        public string CustomerId { get; set; } = string.Empty;
        [Required(ErrorMessage = "Vui lòng nhập tiêu đề thông báo")]
        public string Title { get; set; } = string.Empty;
        [Required(ErrorMessage = "Vui lòng nhập nội dung thông báo")]
        public string Content { get; set; } = string.Empty;
    }
}
