using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Domain.Entities
{
    public class ServiceBooking 
    {
        [ForeignKey("Booking")]
        public string BookingID { get; set; } = string.Empty;

        [ForeignKey("Service")]
        public string ServiceID { get; set; } = string.Empty;
        public int Quantity { get; set; } 
        public virtual Booking ? Booking { get; set; }
        public virtual Service ? Service { get; set; }
    }
}
