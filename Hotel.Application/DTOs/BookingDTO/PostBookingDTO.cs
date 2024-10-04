

using Hotel.Application.DTOs.BookingDetailDTO;
using System.ComponentModel.DataAnnotations;

namespace Hotel.Application.DTOs.BookingDTO
{
    public class PostBookingDTO
    {
        public string? EmployeeId { get; set; }
        [Required]
        public string CustomerId { get; set; } = string.Empty;
        [Required]
        public DateTime CheckInDate { get; set; }
        [Required]
        public DateTime CheckOutDate { get; set; }
        public virtual ICollection<PostBookingDetailDTO> ? BookingDetails { get; set; }
    }
}
