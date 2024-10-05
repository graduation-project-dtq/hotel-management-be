using System.ComponentModel.DataAnnotations;

namespace Hotel.Application.DTOs.HouseTypeDTO
{
    public class PostHouseTypeDTO
    {
        [Required(ErrorMessage ="Không được để trống tên loại")]
        public string Name { get; set; } = string.Empty;
        public string ? Description { get; set; } = string.Empty;
    }
}
