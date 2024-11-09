using Hotel.Application.DTOs.ImageDTO;
using Hotel.Application.DTOs.RoomTypeDetailDTO;
using Hotel.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Application.DTOs.RoomTypeDTO
{
    public class GetRoomTypeDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public float? AverageStart { get; set; }
        //public virtual ICollection<GetRoomTypeDetailDTO>? RoomTypeDetails { get; set; }
        public virtual ICollection<GetImageRoomTypeDTO>? ImageRoomTypes { get; set; }
    }
}
