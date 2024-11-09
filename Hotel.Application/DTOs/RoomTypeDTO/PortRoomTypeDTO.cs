using System.ComponentModel.DataAnnotations;

namespace Hotel.Application.DTOs.RoomTypeDTO
{
    public class PortRoomTypeDTO
    {
        [Required(ErrorMessage = "Vui lòng nhập tên")]
        public string Name { get; set; } = string.Empty;
        public string ? Description { get; set; }

        //public ICollection<>
    }

}
