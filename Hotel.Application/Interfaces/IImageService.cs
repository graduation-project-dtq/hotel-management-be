
using Hotel.Application.DTOs.ImageDTO;
using Hotel.Application.PaggingItems;

namespace Hotel.Application.Interfaces
{
    public interface IImageService
    {
        Task<PaginatedList<GetImageDTO>> GetPageAsync(int index, int pageSize, string idSearch);
    }


}
