namespace Hotel.Application.DTOs.RoomDTO
{
    public class FindRoomDTO
    {
        public DateOnly CheckInDate {  get; set; }
        public DateOnly CheckOutDate { get; set; }
        public string RoomTypeDetailID { get; set; } =string.Empty;
    }
}
