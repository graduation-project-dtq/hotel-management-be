using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Hotel.Application.DTOs.EvaluationDTO
{
    public class PostEvaluationDTO
    {
        [Required(ErrorMessage ="Không được để trống khách hàng")]
        [FromForm]
        public string CustomerId { get; set; } = string.Empty;
        [Required(ErrorMessage = "Không được để trống loại phòng")]
        [FromForm]
        public string RoomTypeId { get; set; } = string.Empty;
        [Required(ErrorMessage = "Vui lòng nhập ý kiến góp ý của bạn")]
        [FromForm]
        public string Comment { get; set; } = string.Empty;
        [Required(ErrorMessage = "Không được để trống số sao")]
        [FromForm]
        public float Starts { get; set; }
    }
}
