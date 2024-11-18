

using Hotel.Application.DTOs.FacilitiesDTO;
using Hotel.Domain.Enums.EnumRoom;

namespace Hotel.Application.DTOs.RoomDTO
{
    public class GetRoomDTO
    {
        public string ? Id {  get; set; }
        public string ? RoomTypeDetailId { get; set; }
        public string ? FloorID { get; set; }
        public string ? Name { get; set; }
        public EnumRoom? Status { get; set; } = EnumRoom.Uninhabited;

        public ICollection<GetFacilitiesRoomDTO>? FacilitiesRoomDTOs { get; set; }
    }
}
