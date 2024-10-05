namespace Hotel.Application.DTOs.HouseTypeDTO
{
    public class GetHoustTypeDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ? Description { get; set; } = string.Empty;
        public virtual ICollection<GetRoomHouseTypeDTO>? Rooms { get; set; }
    }
}
