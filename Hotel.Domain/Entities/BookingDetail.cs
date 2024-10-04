using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Domain.Entities
{
    public class BookingDetail 
    {
        [ForeignKey("Booking")]
        public string BookingId { get; set; }

        [ForeignKey("RoomID")]
        public string ? RoomID { get; set; }
        public decimal ? Price { get; set; }
        public virtual Booking ? Booking { get; set; }
        public virtual Room ? Room { get; set; }
    }
}
