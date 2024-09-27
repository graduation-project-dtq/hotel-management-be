

using Hotel.Core.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Domain.Entities
{
    public class RoomTypeDetail: BaseEntity
    {
        [ForeignKey("RoomType")]
        public string RoomTypeID {  get; set; }
        public string Name { get; set; }
        public int CapacityMax { get; set; }
        public decimal Area { get; set; }
        public string Description { get; set; }
        public float AverageStart { get; set; }
        public virtual RoomType ? RoomType { get; set; }
        public virtual ICollection<Booking> ? Bookings { get; set; }

        public virtual ICollection<ImageRoomTypeDetail> ? ImageRoomTypeDetails { get; set; }
    }
}
