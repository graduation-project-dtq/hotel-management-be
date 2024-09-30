using Hotel.Application.DTOs.BookingDetailDTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Application.DTOs.BookingDTO
{
    public class GetBookingDTO
    {
        public string ? Id {  get; set; }
        public string ? EmployeeId { get; set; }
        public string ?CustomerId { get; set; }
        public DateTime ? BookingDate { get; set; }
        public DateTime ? CheckInDate { get; set; }
        public DateTime ? CheckOutDate { get; set; }
        public virtual ICollection<GetBookingDetailDTO> ? BookingDetail { get; set; }

    }
}
