using Hotel.Core.Base;
namespace Hotel.Domain.Entities
{
    public class RoomPrice : BaseEntity
    {
        public decimal BasePrice { get; set; }
        public int Capacity { get; set; }
        public string Description { get; set; }
        public virtual ICollection<RoomTypeDetail> ? RoomTypeDetail { get; set; }
        public virtual ICollection<RoomPriceAdjustment> ? RoomPriceAdjustments { get; set; }
    }
}
