using Hotel.Core.Base;
namespace Hotel.Domain.Entities
{
    public class RoomPrice : BaseEntity
    {
        public string Name {  get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public string Description { get; set; } = string.Empty;
        public virtual ICollection<RoomTypeDetail> ? RoomTypeDetail { get; set; }
        public virtual ICollection<RoomPriceAdjustment> ? RoomPriceAdjustments { get; set; }
    }
}
