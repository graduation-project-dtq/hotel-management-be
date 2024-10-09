using Hotel.Application.DTOs.BookingDetailDTO;
using Hotel.Application.DTOs.ServiceDTO;
using Hotel.Domain.Entities;


namespace Hotel.Application.DTOs.BookingDTO
{
    public class GetBookingDTO
    {
        public string ? Id {  get; set; }
        public string ? EmployeeId { get; set; }
        public string ?CustomerId { get; set; }
        public DateOnly? BookingDate { get; set; }
        public DateOnly? CheckInDate { get; set; }
        public DateOnly? CheckOutDate { get; set; }
        public virtual ICollection<GetBookingDetailDTO> ? BookingDetail { get; set; }
        public virtual ICollection<GetServiceBookingDTO>? Services {  get; set; }
 
    }
}
