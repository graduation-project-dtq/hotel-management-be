using Hotel.Application.DTOs.BookingDTO;
using Hotel.Domain.Entities;

namespace Hotel.Application.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendBookingConfirmationEmailAsync(Booking booking, string customerId, GetBookingDTO bookingDTO);
    }
}
