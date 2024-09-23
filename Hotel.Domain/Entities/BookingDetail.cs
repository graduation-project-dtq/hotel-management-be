
using Hotel.Core.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Domain.Entities
{
    public class BookingDetail : BaseEntity
    {
        [ForeignKey("Booking")]
        public string BookingId { get; set; }

        [ForeignKey("Room")]
        public string RoomId { get; set; }
        public decimal Price { get; set; }
        public virtual Booking ? Booking { get; set; }
        public virtual Room ? Room { get; set; }
    }
}
