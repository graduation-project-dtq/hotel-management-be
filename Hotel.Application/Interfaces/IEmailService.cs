using Hotel.Application.DTOs.BookingDTO;
using Hotel.Domain.Entities;

namespace Hotel.Application.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendBookingConfirmationEmailAsync(Booking booking, string customerId, GetBookingDTO bookingDTO);
        Task SendEmailAsync(string recipientEmail, bool isConfirmation, GetBookingDTO model, int count);
    }
}
