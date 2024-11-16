
namespace Hotel.Application.DTOs.RoomDTO
{
    public class PutRoomDTO
    {
        public string ? Name {  get; set; }
        public ICollection<PostFacilitiesRoomDTO>? Facilities { get; set; }
    }
}
