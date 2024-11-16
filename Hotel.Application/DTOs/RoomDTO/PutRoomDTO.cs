
using System.ComponentModel.DataAnnotations;

namespace Hotel.Application.DTOs.RoomDTO
{
    public class PutRoomDTO
    {
       
        public string ? RoomTypeDetailId { get; set; } 
        public string? FloorID { get; set; }
        public string ? Name {  get; set; }
        public ICollection<PostFacilitiesRoomDTO>? Facilities { get; set; }
    }
}
