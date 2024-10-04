
using Hotel.Application.DTOs.BookingDTO;
using Hotel.Application.DTOs.RoomDTO;
using Hotel.Application.PaggingItems;

namespace Hotel.Application.Interfaces
{
    public interface IBookingService
    {
      
        Task<PaginatedList<GetBookingDTO>> GetPageAsync(int index, int pageSize, string idSearch, string customerID,string employeeID);
        //Task<GetBookingDTO> CreateBooking(PostBookingDTO model);
    }
}
