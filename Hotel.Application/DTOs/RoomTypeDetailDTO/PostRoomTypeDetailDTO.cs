
namespace Hotel.Application.DTOs.RoomTypeDetailDTO
{
    public class PostRoomTypeDetailDTO
    {
        public string RoomTypeID { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int CapacityMax { get; set; }
        public decimal Area { get; set; }
        public string? Description { get; set; }
    }
}
