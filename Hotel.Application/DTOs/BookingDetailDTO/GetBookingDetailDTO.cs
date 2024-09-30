
using Hotel.Application.DTOs.RoomDTO;

namespace Hotel.Application.DTOs.BookingDetailDTO
{
    public class GetBookingDetailDTO
    {
        public int Quantity {  get; set; }
        public virtual GetRoomBookingDTO? Room { get; set; }

    }
}
