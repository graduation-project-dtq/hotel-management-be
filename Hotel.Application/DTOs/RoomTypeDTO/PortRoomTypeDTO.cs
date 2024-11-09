using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Application.DTOs.RoomTypeDTO
{
    public class PortRoomTypeDTO
    {
        [Required(ErrorMessage = "Vui lòng nhập tên")]
        public string Name { get; set; } = string.Empty;

        public string ? Description { get; set; }
    }

}
