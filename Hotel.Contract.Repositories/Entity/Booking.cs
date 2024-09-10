

using Hotel.Core.Base;
using System.ComponentModel.DataAnnotations;

namespace Hotel.Contract.Repositories.Entity
{
    public class Booking:BaseEntity
    {
        [StringLength(50)]    
        public string EmployeeId { get; set; }
        [StringLength(50)]
        public string CustomerId { get; set; }
        [StringLength(50)]
        public string RoomTypeDetailId { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual RoomTypeDetail RoomTypeDetail { get; set; }
        public virtual ICollection<BookingDetail> BookingDetails { get; set; }
        public virtual ICollection<ServiceBooking> ServiceBookings { get; set; }

    }
}
