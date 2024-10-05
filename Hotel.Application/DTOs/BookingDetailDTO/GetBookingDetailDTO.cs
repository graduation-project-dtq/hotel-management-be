
using Hotel.Application.DTOs.RoomDTO;

namespace Hotel.Application.DTOs.BookingDetailDTO
{
    public class GetBookingDetailDTO
    {
        public string RoomID {  get; set; }=string.Empty;
        public virtual GetRoomBookingDTO? Room { get; set; }
    }
}
