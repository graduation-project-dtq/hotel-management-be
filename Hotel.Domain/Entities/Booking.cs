﻿using Hotel.Core.Base;
using Hotel.Domain.Enums.EnumBooking;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Domain.Entities
{
    public class Booking : BaseEntity
    {
        [ForeignKey("Employee")]
        public string ? EmployeeId { get; set; }

        [ForeignKey("Customer")]
        public string CustomerId { get; set; } =string .Empty;
        public DateOnly BookingDate { get; set; }
        public DateOnly CheckInDate { get; set; }
        public DateOnly CheckOutDate { get; set; }
        public decimal TotalAmount { get; set; }
        public EnumBooking Status { get; set; }
        public virtual Customer ? Customer { get; set; }
        public virtual Employee ? Employee { get; set; }
        public virtual ICollection<BookingDetail> ? BookingDetails { get; set; }
        public virtual ICollection<ServiceBooking> ? ServiceBookings { get; set; }
        public virtual ICollection<Punish>  ? Punishes { get; set; }
    }
}
