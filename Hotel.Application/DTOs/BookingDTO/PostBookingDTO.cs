using Hotel.Application.DTOs.BookingDetailDTO;
using Hotel.Application.DTOs.ServiceDTO;
using System.ComponentModel.DataAnnotations;

namespace Hotel.Application.DTOs.BookingDTO
{
    public class PostBookingDTO
    {
        public string? EmployeeId { get; set; }
        [Required]
        public string CustomerId { get; set; } = string.Empty;
        [Required]
        public DateOnly CheckInDate { get; set; }
        [Required]
        public DateOnly CheckOutDate { get; set; }
        public string ? VoucherId {  get; set; }

        //Tiền đặt cọc
        public decimal  Deposit { get; set; }
        [Phone]
        public string PhoneNumber {  get; set; }
        public virtual ICollection<PostBookingDetailDTO> ? BookingDetails { get; set; }
        public virtual ICollection<PostServiceBookingDTO> ? Services {  get; set; }
    }
}
