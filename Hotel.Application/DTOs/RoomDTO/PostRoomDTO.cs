
using System.ComponentModel.DataAnnotations;

namespace Hotel.Application.DTOs.RoomDTO
{
    public class PostRoomDTO
    {
        [Required(ErrorMessage ="Vui lòng chọn loại phòng")]
        public string RoomTypeDetailId { get; set; } = string.Empty;
        [Required(ErrorMessage = "Vui lòng chọn tầng")]
        public string? FloorID { get; set; } = string.Empty;
        [Required(ErrorMessage = "Vui lòng nhập tên phòng")]
        public string Name { get; set; } = string.Empty;

        public ICollection<PostFacilitiesRoomDTO>? Facilities { get; set; }
    }
}
