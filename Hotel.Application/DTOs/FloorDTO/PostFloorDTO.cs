

using System.ComponentModel.DataAnnotations;

namespace Hotel.Application.DTOs.FloorDTO
{
    public class PostFloorDTO
    {
        [Required(ErrorMessage ="Vui lòng nhập tên")]
        public string Name { get; set; } = string.Empty;
    }
}
