using Hotel.Application.DTOs.ImageDTO;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Hotel.Application.DTOs.RoomTypeDTO
{
    public class PortRoomTypeDTO
    {
        [Required(ErrorMessage = "Vui lòng nhập tên")]
        [FromForm]
        public string Name { get; set; } = string.Empty;
        [FromForm]
        public string ? Description { get; set; }

    }

}
