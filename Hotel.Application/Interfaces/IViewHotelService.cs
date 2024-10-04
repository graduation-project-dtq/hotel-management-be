
using Hotel.Application.DTOs.ViewHotelDTO;
using Hotel.Application.PaggingItems;

namespace Hotel.Application.Interfaces
{
    public interface IViewHotelService
    {
        Task<PaginatedList<GetViewHotelDTO>> GetPageAsync(int index, int pageSize, string idSearch);
        Task DeleteViewHotel(string id);
        Task<GetViewHotelDTO> CreateViewHotel(PostViewHotelDTO model);
        Task<GetViewHotelDTO> UpdateViewHotel(string id, PutViewHotelDTO model);
    }
}
