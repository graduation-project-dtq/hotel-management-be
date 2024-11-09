

using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Hotel.Application.DTOs.FacilitiesDTO
{
    public class PostFacilitiesDTO
    {
        [Required(ErrorMessage ="Vui lòng nhập tên")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Vui lòng nhập giá tiền")]
        public decimal Price { get; set; }
        public string? Description { get; set; }
    }
}
