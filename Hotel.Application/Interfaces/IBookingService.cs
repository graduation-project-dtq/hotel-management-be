
using Hotel.Application.DTOs.BookingDTO;
using Hotel.Application.PaggingItems;
using Hotel.Domain.Enums.EnumBooking;

namespace Hotel.Application.Interfaces
{
    public interface IBookingService
    {

        Task<PaginatedList<GetBookingDTO>> GetPageAsync(int index, int pageSize, string idSearch, string customerID
            , string customerName, DateOnly? bookingDate, DateOnly? checkInDate, EnumBooking? status, string phone);
         Task<GetBookingDTO> CreateBooking(PostBookingDTO model);
        Task UpdateStatusBooking(string bookingID); //Xác nhận hoặc huỷ dựa vào Status hiệnt tại của booking
        Task<List<GetBookingDTO>> GetBookingByCustomerId(string CustomerId, EnumBooking enumBooking);
        Task CheckIn(CheckInDTO model);
        Task CheckOut(CheckOutDTO model);
        Task HuyPhong(string id);

        Task<StatisticaInDTO> StatisticaInDate(DateOnly date);
        Task<StatisticaInDTO> StatisticaInMonth(int month, int year);
        Task<StatisticaInDTO> StatisticaInYear(int year);


    }
}
