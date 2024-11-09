
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Application.DTOs.RoomTypeDetailDTO
{
    public class PostRoomTypeDetailDTO
    {
        [FromForm]
        public string RoomTypeID { get; set; } = string.Empty;
        [FromForm]
        public string Name { get; set; } = string.Empty;
        [FromForm]
        public int CapacityMax { get; set; }
        [FromForm]
        public decimal Area { get; set; }
        [FromForm]
        public string? Description { get; set; }
    }
}
