
using Hotel.Application.DTOs.BookingDTO;
using Hotel.Application.PaggingItems;
using Hotel.Domain.Enums.EnumBooking;

namespace Hotel.Application.Interfaces
{
    public interface IBookingService
    {
      
        Task<PaginatedList<GetBookingDTO>> GetPageAsync(int index, int pageSize, string idSearch, string customerID,string employeeID, DateOnly? bookingDate, DateOnly ?  checkInDate);
        Task<GetBookingDTO> CreateBooking(PostBookingDTO model);
        Task UpdateStatusBooking(string bookingID); //Xác nhận hoặc huỷ dựa vào Status hiệnt tại của booking
        Task<List<GetBookingDTO>> GetBookingByCustomerId(string CustomerId, EnumBooking enumBooking);
        Task CheckIn(CheckInDTO model);
        Task CheckOut(CheckOutDTO model);
        Task HuyPhong(string id);
    }
}
