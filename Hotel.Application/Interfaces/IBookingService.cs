
using Hotel.Application.DTOs.BookingDTO;

namespace Hotel.Application.Interfaces
{
    public interface IBookingService
    {
        Task<List<GetBookingDTO>> GetAllBooking();
        Task<GetBookingDTO> CreateBooking(PostBookingDTO model);
    }
}
