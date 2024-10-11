

using Hotel.Core.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Domain.Entities
{
    public class RoomTypeDetail: BaseEntity
    {
        [ForeignKey("RoomType")]
        public string RoomTypeID {  get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int CapacityMax { get; set; }
        public decimal Area { get; set; }
        public decimal BasePrice {  get; set; } //Giá cơ bản
        public string ? Description { get; set; } = string.Empty;
        public float AverageStart { get; set; }
        public virtual RoomType ? RoomType { get; set; }
        public virtual ICollection<ImageRoomTypeDetail> ? ImageRoomTypeDetails { get; set; }
        public virtual ICollection<VoucherRoomTypeDetail>? VoucherRoomTypeDetails { get; set; }
        public virtual ICollection<RoomPriceAdjustment> ? RoomPriceAdjustments { get; set; }
    }
}
