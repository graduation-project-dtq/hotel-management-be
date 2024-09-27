using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Domain.Entities
{
    public class ServiceBooking 
    {
        [ForeignKey("Booking")]
        public string BookingID { get; set; }

        [ForeignKey("Service")]
        public string ServiceID { get; set; }
        public int Quantity { get; set; }
        public float Total { get; set; }

        public virtual Booking ? Booking { get; set; }
        public virtual Service ? Service { get; set; }
    }
}
