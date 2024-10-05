using Hotel.Application.DTOs.BookingDetailDTO;


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

    }
}
