
using Hotel.Application.DTOs.RoomDTO;
using System.Text.Json.Serialization;

namespace Hotel.Application.DTOs.BookingDetailDTO
{
    public class GetBookingDetailDTO
    {
        public string RoomID {  get; set; }=string.Empty;

        public string? RoomName { get; set; }
        [JsonIgnore]
        public virtual GetRoomBookingDTO? Room { get; set; }
    }
}
