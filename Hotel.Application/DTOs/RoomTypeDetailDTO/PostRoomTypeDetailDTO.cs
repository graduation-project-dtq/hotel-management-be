
namespace Hotel.Application.DTOs.RoomTypeDetailDTO
{
    public class PostRoomTypeDetailDTO
    {
        public string? RoomTypeID { get; set; }
        public string? Name { get; set; }
        public int? CapacityMax { get; set; }
        public decimal? Area { get; set; }
        public string? Description { get; set; }
        public float? AverageStart { get; set; }
    }
}
