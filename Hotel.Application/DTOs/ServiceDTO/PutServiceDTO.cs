using Hotel.Application.DTOs.ImageDTO;
using System.ComponentModel.DataAnnotations;

namespace Hotel.Application.DTOs.ServiceDTO
{
    public class PutServiceDTO
    {

        [Required(ErrorMessage = "Không được để trống tên")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Không được để trống giá tiền")]
        public decimal Price { get; set; }
        public string? Description { get; set; } = string.Empty;
        public virtual ICollection<PostImageServiceDTO>? PostImageServiceDTOs { get; set; }
    }
}
