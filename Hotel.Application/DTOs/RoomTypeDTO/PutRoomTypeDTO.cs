using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Application.DTOs.RoomTypeDTO
{
    public class PutRoomTypeDTO
    {
        [FromForm]
        public string? Name { get; set; } = string.Empty;
        [FromForm]
        public string? Description { get; set; }
    }
}
