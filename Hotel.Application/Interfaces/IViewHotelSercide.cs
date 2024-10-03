
using Hotel.Application.DTOs.ViewHotelDTO;
using Hotel.Application.PaggingItems;

namespace Hotel.Application.Interfaces
{
    public interface IViewHotelSercide
    {
        Task<PaginatedList<GetViewHotelDTO>> GetPageAsync(int index, int pageSize, string idSearch);
    }
}
