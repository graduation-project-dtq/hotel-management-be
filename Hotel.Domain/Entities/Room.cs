using Hotel.Core.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Domain.Entities
{
    public class Room : BaseEntity
    {
        [ForeignKey("Floor")]
        public string FloorId { get; set; }

        
        [ForeignKey("RoomTypeDetail")]
        public string RoomTypeDetailId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
        public virtual Floor ? Floor { get; set; }
        public virtual RoomTypeDetail ? RoomTypeDetail { get; set; }
        public virtual ICollection<BookingDetail> ? BookingDetails { get; set; }
        public virtual ICollection<RoomView> ? RoomViews { get; set; }
        public virtual ICollection<FacilitiesRoom> ? FacilitiesRooms { get; set; }

    }
}
